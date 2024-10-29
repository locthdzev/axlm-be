using System.Net;
using AutoMapper;
using Data.Models.FilterModel;
using Data.Models.ResultModel;
using Data.Utilities.Email;
using Data.Utilities.Pagination;
using ConvertUlti = Data.Utilities.Convert.Convert;
using Data.Entities;
using Encoder = Data.Utilities.Encoder.Encoder;
using static Data.Enums.Status;
using Repositories.UserRepositories;
using Data.Models.UserModel;

namespace UserAPIServices.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        // private readonly IStudentClassRepository _studentClassRepository;
        // private readonly IAttendanceRepo _AttendanceRepo;
        // private readonly ICertificateRepo _certificateRepository;
        private readonly IEmail _email;

        public UserServices(IUserRepository userRepository, IEmail email)
        {
            _userRepository = userRepository;
            // _studentClassRepository = studentClassRepository;
            // _AttendanceRepo = AttendanceRepo;
            // _certificateRepository = certificateRepository;
            _email = email;
        }


        public async Task<ResultModel> CreateAccount(UserCreateReqModel CreateForm)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(CreateForm.Email);
                if (User != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Email is existed!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserCreateReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore()); ;
                    });
                    IMapper mapper = config.CreateMapper();
                    User NewUser = mapper.Map<UserCreateReqModel, User>(CreateForm);
                    if (CreateForm.Password == null)
                    {
                        CreateForm.Password = Encoder.GenerateRandomPassword();
                    }
                    // string FilePath = "../Business/TemplateEmail/FirstInformation.html";
                    string FilePath = "../Data/Utilities/TemplateEmail/FirstInformation.html";
                    string Html = File.ReadAllText(FilePath);
                    Html = Html.Replace("{{Password}}", CreateForm.Password);
                    Html = Html.Replace("{{Email}}", CreateForm.Email);
                    bool check = await _email.SendEmail(CreateForm.Email, "Login Information", Html);
                    var HashedPasswordModel = Encoder.CreateHashPassword(CreateForm.Password);
                    NewUser.Password = HashedPasswordModel.HashedPassword;
                    NewUser.Salt = HashedPasswordModel.Salt;
                    NewUser.Status = UserStatus.ACTIVE;
                     await _userRepository.Insert(NewUser);
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Message = "Create account successfully!";
                }
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetUserProfile(Guid userId)
        {
            ResultModel Result = new();
            try
            {
                var user = await _userRepository.GetUserById(userId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "Not found";
                    return Result;
                }

                var userProfile = new
                {
                    User_ID = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    Dob = user.Dob,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    Role = user.Role,

                };

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = userProfile;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }



        public async Task<ResultModel> GetAllUser(int page, FilterModel reqModel)
        {
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var users = await _userRepository.GetAllUser(reqModel);

                if (users == null || !users.Any())
                {
                    return new ResultModel { IsSuccess = false, Message = "No user found", Code = 404 };
                }

                var userProfiles = users.Select(user => new UserResModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    Dob = user.Dob,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    Role = user.Role,
                    Status = user.Status
                }).ToList();

                var exceptList = new List<UserResModel>();
                if (reqModel.searchValue != null)
                {
                    foreach (var user in userProfiles)
                    {
                        if (!ConvertUlti.ConvertToUnsign(user.FullName).Contains(reqModel.searchValue, StringComparison.OrdinalIgnoreCase) &&
                            !user.FullName.Contains(reqModel.searchValue, StringComparison.OrdinalIgnoreCase) &&
                            !user.Email.Contains(reqModel.searchValue, StringComparison.OrdinalIgnoreCase))
                        {
                            exceptList.Add(user);
                        }
                    }
                }

                if (exceptList.Count > 0)
                {
                    userProfiles = userProfiles.Except(exceptList).ToList();
                }

                var ResultList = await Pagination.GetPagination(userProfiles, page, 10);
                return new ResultModel { IsSuccess = true, Data = ResultList, Code = 200 };
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, Message = ex.Message, Code = 400 };
            }
        }

        // public async Task<ResultModel> ViewAccountsInfo(Guid userId)
        // {
        //     ResultModel resultModel = new ResultModel();
        //     try
        //     {
        //         var userInfo = await _userRepository.GetAccountInfo(userId);
        //         if (userInfo.Role.Equals(Roles.STUDENT))
        //         {
        //             resultModel.Data = new StudentAccountInfoModel
        //             {
        //                 userResModel = userInfo,
        //                 ClassInformationOfStudent = await _studentClassRepository.GetClassInfoByStudentId(userId)
        //             };
        //         }
        //         else
        //         {
        //             resultModel.Data = new OtherAccountInfoModel
        //             {
        //                 userResModel = userInfo,
        //                 classInformationOfOthers = await _studentClassRepository.GetClassInfoByOtherId(userId)
        //             };
        //         }

        //         resultModel.IsSuccess = true;
        //         resultModel.Code = (int)HttpStatusCode.OK;
        //     }
        //     catch (Exception ex)
        //     {
        //         resultModel.IsSuccess = false;
        //         resultModel.Code = (int)HttpStatusCode.BadRequest;
        //         resultModel.Message = ex.Message;
        //     }
        //     return resultModel;
        // }

        public async Task<ResultModel> GetTrainerList(int page)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var trainerList = await _userRepository.GetTrainerList();
                var resultList = await Pagination.GetPagination(trainerList, page, 10);

                resultModel.Data = resultList;
                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }


        // public async Task<ResultModel> GetAttendanceById(Guid attendanceId)
        // {
        //     ResultModel resultModel = new ResultModel();
        //     try
        //     {
        //         var attendance = await _AttendanceRepo.GetAttendanceById(attendanceId);
        //         resultModel.Data = attendance;
        //         resultModel.IsSuccess = true;
        //         resultModel.Code = (int)HttpStatusCode.OK;
        //     }
        //     catch (Exception ex)
        //     {
        //         resultModel.IsSuccess = false;
        //         resultModel.Code = (int)HttpStatusCode.BadRequest;
        //         resultModel.Message = ex.Message;
        //     }
        //     return resultModel;
        // }

        public async Task<ResultModel> UpdateAccountsStatus(UpdateAccountsStatusModel reqModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (reqModel.UserId.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "The input is null";
                    return result;
                }
                else
                {
                    var updateList = await _userRepository.GetUserListByListId(reqModel.UserId);
                    foreach (var update in updateList)
                    {
                        update.Status = reqModel.status;
                    }
                    await _userRepository.UpdateRange(updateList);

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = $"Users status is successfully changed to {reqModel.status}";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.Message;
            }
            return result;
        }
    }
}