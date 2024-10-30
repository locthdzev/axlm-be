using AutoMapper;
using Data.Entities;
using Data.Models.FilterModel;
using Data.Models.NotificationModel;
using Data.Models.ResultModel;
using Data.Utilities.CloudStorage;
using Data.Utilities.Pagination;
using Repositories.NotificationRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace NotificationAPIServices.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICloudStorage _firebaseStorage;

        public NotificationServices(INotificationRepository notificationRepository, IUserRepository userRepository, ICloudStorage firebaseStorage)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<ResultModel> CreateNotification(string token, NotificationCreateReqModel notificationCreateReqModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var user = _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found!";
                    return result;
                }
                Guid Id = Guid.NewGuid();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<NotificationCreateReqModel, Notification>();
                });
                IMapper mapper = config.CreateMapper();
                Notification newNotification = mapper.Map<NotificationCreateReqModel, Notification>(notificationCreateReqModel);

                newNotification.Id = Id;
                newNotification.Title = notificationCreateReqModel.Title;
                newNotification.Description = notificationCreateReqModel.Description;
                newNotification.CreatedBy = userId;
                newNotification.CreatedAt = DateTime.Now;
                string filePath = $"notification/{Id}";
                if (notificationCreateReqModel.File != null)
                {
                    var fileName = await _firebaseStorage.UploadOneFileToFirebase(notificationCreateReqModel.File, filePath);
                    newNotification.Attachment = fileName;
                }
                newNotification.Status = GeneralStatus.ACTIVE;
                await _notificationRepository.Insert(newNotification);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Notification created successfully!";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetAllNotifications(string token, int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var user = await _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found!";
                    return result;
                }

                if (page == null || page == 0)
                {
                    page = 1;
                }

                var notifications = await _notificationRepository.GetAllNotifications();
                List<NotificationResModel> notificationResModels = new();
                FilterModel reqModel = new();
                var AllUser = await _userRepository.GetAllUser(reqModel);
                foreach (var n in notifications)
                {

                    NotificationResModel nt = new()
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Description = n.Description,
                        Attachment = n.Attachment,
                        CreatedAt = n.CreatedAt,
                        CreatedBy = AllUser.Where(x => x.Id.Equals(n.CreatedBy)).Select(u => new AuthorNotificationResModel
                        {
                            Id = u.Id,
                            Name = u.FullName
                        }).FirstOrDefault(),
                        Status = n.Status
                    };
                    notificationResModels.Add(nt);
                }

                var ResultList = await Pagination.GetPagination(notificationResModels, page, 6);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = ResultList;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetNotificationDetails(Guid notificationId, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var user = _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found!";
                    return result;
                }
                FilterModel reqModel = new();
                var AllUser = await _userRepository.GetAllUser(reqModel);

                var notification = await _notificationRepository.Get(notificationId);


                var notificationDetails = new
                {
                    ID = notification.Id,
                    Title = notification.Title,
                    Description = notification.Description,
                    Attachment = notification.Attachment,
                    Status = notification.Status,
                    CreatedAt = notification.CreatedAt,
                    CreatedBy = AllUser.Where(x => x.Id.Equals(notification.CreatedBy)).Select(u => new AuthorNotificationResModel
                    {
                        Id = u.Id,
                        Name = u.FullName
                    }).FirstOrDefault(),

                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = notificationDetails;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateNotification(Guid notificationId, string token, NotifcationUpdateModel notificationUpdateModel)
        {
            ResultModel Result = new();
            try
            {
                var notification = await _notificationRepository.Get(notificationId);

                if (notificationUpdateModel is null)
                {
                    throw new ArgumentException(nameof(notificationUpdateModel));
                }

                string filePath = $"notification/{notification.Id}";

                await _firebaseStorage.RemoveFileFromFirebase(notification.Attachment, filePath);

                notification.Title = notificationUpdateModel.Title;
                notification.Description = notificationUpdateModel.Description;
                List<IFormFile> files = new List<IFormFile>();
                files.Add(notificationUpdateModel.File);
                if (notificationUpdateModel.File != null)
                {
                    var fileName = await _firebaseStorage.UploadFilesToFirebase(files, filePath);
                    notification.Attachment = fileName[0];
                }
                notification.Status = GeneralStatus.ACTIVE;

                await _notificationRepository.Update(notification);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Notification updated successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return Result;
        }

        public async Task<ResultModel> DeleteNotification(Guid notificationId, string token)
        {
            ResultModel Result = new();
            try
            {
                Guid UserId = new Guid(Encoder.DecodeToken(token, "userid"));
                var GetNotification = await _notificationRepository.Get(notificationId);
                if (GetNotification == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "Notification is not found";
                    return Result;
                }
                else if (!GetNotification.CreatedBy.Equals(UserId))
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "You can't delete this notification";
                    return Result;
                }

                string FilePath = $"notification/{GetNotification.Id}";
                await _firebaseStorage.RemoveFileFromFirebase(GetNotification.Attachment, FilePath);
                await _notificationRepository.Remove(GetNotification);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Delete Notification successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }
    }
}