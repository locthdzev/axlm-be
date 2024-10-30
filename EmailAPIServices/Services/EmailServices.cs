using AutoMapper;
using Data.Entities;
using Data.Models.EmailModel;
using Data.Models.ResultModel;
using Data.Utilities.CloudStorage;
using Data.Utilities.Pagination;
using Repositories.EmailRepositories;
using Repositories.RequestReplyRepositories;
using Repositories.StudentClassRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace EmailAPIServices.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IRequestReplyRepository _requestReplyRepository;
        private readonly ICloudStorage _firebaseStorage;

        public EmailServices(IEmailRepository emailRepository, IUserRepository userRepository, IStudentClassRepository studentClassRepository, IRequestReplyRepository requestReplyRepository, ICloudStorage firebaseStorage)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _studentClassRepository = studentClassRepository;
            _requestReplyRepository = requestReplyRepository;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<ResultModel> GetEmailRequests(Guid userId, int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var emails = await _emailRepository.GetEmailRequestsByUserId(userId);
                List<EmailResModel> emailRequestList = new();
                foreach (var e in emails)
                {
                    var Recipient = await _userRepository.GetUserById(e.RecipientId);
                    RecipientModel RecipientBy = new()
                    {
                        Id = Recipient.Id,
                        FullName = Recipient.FullName,
                        Email = Recipient.Email,
                    };

                    var Creator = await _userRepository.GetUserById(e.CreatedBy);
                    CreatorEmailModel CreatorBy = new()
                    {
                        Id = Creator.Id,
                        FullName = Creator.FullName,
                        Email = Creator.Email,
                    };

                    EmailResModel em = new()
                    {
                        Id = e.Id,
                        Recipient = RecipientBy,
                        Subject = e.Subject,
                        Content = e.Content,
                        Attachment = e.Attachment,
                        CreatedAt = e.CreatedAt,
                        CreatedBy = CreatorBy,
                        Status = e.Status,
                    };
                    emailRequestList.Add(em);

                }
                if (emailRequestList.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404
                    };
                }

                var ResultList = await Pagination.GetPagination(emailRequestList, page, 10);

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

        public async Task<ResultModel> SendEmail(string recipientEmail, string token, EmailCreateReqModel form)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var config = new MapperConfiguration(cfg => cfg.CreateMap<EmailCreateReqModel, EmailRequest>());

                Guid recipientId = await _userRepository.GetUserIdByEmail(recipientEmail);

                if (recipientId == Guid.Empty)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    resultModel.Message = "This user is not found!";
                    return resultModel;
                }

                IMapper mapper = config.CreateMapper();
                EmailRequest newEmail = mapper.Map<EmailCreateReqModel, EmailRequest>(form);

                var emailID = Guid.NewGuid();
                newEmail.Id = emailID;
                newEmail.RecipientId = recipientId;
                if (form.Files != null)
                {
                    string fileName = await _firebaseStorage.UploadOneFileToFirebase(form.Files, $"Mail_{emailID}");
                    newEmail.Attachment = fileName;
                }
                newEmail.CreatedAt = DateTime.Now;
                newEmail.CreatedBy = userId;
                newEmail.Status = GeneralStatus.ACTIVE;
                await _emailRepository.Insert(newEmail);

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Message = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.ResponseFailed = ex.InnerException != null ? ex.InnerException.Message + "\n" + ex.StackTrace : ex.Message + "\n" + ex.StackTrace;
            }
            return resultModel;
        }

        public async Task<ResultModel> ViewEmailAndReplies(Guid emailId, string token)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));

                var emailRequest = await _emailRepository.GetEmailById(emailId);
                if (emailRequest == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    resultModel.Message = "Email request not found";
                    return resultModel;
                }

                var emailDetails = new Dictionary<string, object>
                {
                    { "Subject", emailRequest.Subject },
                    { "Content", emailRequest.Content },
                    { "Attachment", emailRequest.Attachment }
                };

                var replies = await _requestReplyRepository.GetRepliesByEmailId(emailId);
                var sortedReplies = replies.OrderBy(reply => reply.CreatedAt).ToList();

                if (sortedReplies.Count > 0)
                {
                    var replyList = new List<ReplyResModel>();
                    foreach (var reply in sortedReplies)
                    {
                        replyList.Add(new ReplyResModel
                        {
                            ReplyContent = reply.ReplyContent,
                            CreatedAt = reply.CreatedAt,
                            Sender = await _userRepository.GetSenderInfoById(reply.CreatedBy),
                        });
                    }
                    emailDetails.Add("Replies", replyList);
                }

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = emailDetails;
                resultModel.Message = "Email viewed successfully!";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.ResponseFailed = ex.InnerException != null ? ex.InnerException.Message + "\n" + ex.StackTrace : ex.Message + "\n" + ex.StackTrace;
            }
            return resultModel;
        }

        public async Task<ResultModel> SendReply(Guid emailId, string replyContent, string token)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));

                var emailRequest = await _emailRepository.GetEmailById(emailId);
                if (emailRequest == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    resultModel.Message = "Email request not found";
                    return resultModel;
                }

                var newReply = new RequestReply
                {
                    Id = Guid.NewGuid(),
                    RequestId = emailId,
                    ReplyContent = replyContent,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    Status = GeneralStatus.ACTIVE
                };

                await _requestReplyRepository.Insert(newReply);

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Message = "Reply sent successfully!";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.ResponseFailed = ex.InnerException != null ? ex.InnerException.Message + "\n" + ex.StackTrace : ex.Message + "\n" + ex.StackTrace;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetEmailReceived(Guid userId, int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var emails = await _emailRepository.GetEmailReceivedByUserId(userId);
                List<EmailResReceivedModel> emailReceiveList = new();
                foreach (var e in emails)
                {
                    var Sender = await _userRepository.GetUserById(e.CreatedBy);
                    SenderEmailModel SenderBy = new()
                    {
                        Id = Sender.Id,
                        FullName = Sender.FullName,
                        Email = Sender.Email,
                    };

                    EmailResReceivedModel em = new()
                    {
                        Id = e.Id,
                        Subject = e.Subject,
                        Content = e.Content,
                        Attachment = e.Attachment,
                        CreatedAt = e.CreatedAt,
                        Sender = SenderBy,
                        Status = e.Status,
                    };
                    emailReceiveList.Add(em);

                }
                if (emailReceiveList.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404
                    };
                }

                var ResultList = await Pagination.GetPagination(emailReceiveList, page, 10);

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

        public async Task<ResultModel> GetRecipientOfStudent(string token)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var result = await _studentClassRepository.GetAdminAndTrainerOfStudent(userId);

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = result;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.Message = e.Message;
            }
            return resultModel;
        }
    }
}