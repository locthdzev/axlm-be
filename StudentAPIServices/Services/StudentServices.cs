using System.Net;
using AutoMapper;
using Data.Entities;
using Data.Enums;
using Data.Models.FilterModel;
using Data.Models.ResultModel;
using Data.Models.SubmissionModel;
using Data.Models.UserModel;
using Data.Utilities.Email;
using Data.Utilities.Pagination;
using Repositories.StudentClassRepositories;
using Repositories.StudentRepositories;
using Repositories.SubmissionRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using ConvertUlti = Data.Utilities.Convert.Convert;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace StudentAPIServices.Services
{
    public class StudentServices : IStudentServices
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmail _email;

        public StudentServices(IStudentRepository studentRepository, IUserRepository userRepository, ISubmissionRepository submissionRepository, IStudentClassRepository studentClassRepository, IEmail email)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _submissionRepository = submissionRepository;
            _studentClassRepository = studentClassRepository;
            _email = email;
        }

        public async Task<ResultModel> GetAllStudents(int page, FilterModel reqModel)
        {
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var students = await _userRepository.GetAllStudents(reqModel);

                if (students == null || !students.Any())
                {
                    return new ResultModel { IsSuccess = false, Message = "No students found", Code = 404 };
                }

                var userProfiles = students.Select(user => new UserResModel
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
        public async Task<ResultModel> CreateStudent(StudentCreateReqModel CreateStudent)
        {
            ResultModel Result = new();
            try
            {
                // Kiểm tra xem email đã tồn tại hay chưa
                var Student = await _studentRepository.GetStudentByEmail(CreateStudent.Email);
                if (Student != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = $"Email {CreateStudent.Email} is existed!";
                    return Result; // Trả về kết quả nếu email đã tồn tại
                }

                // Cấu hình ánh xạ giữa StudentCreateReqModel và User
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<StudentCreateReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore());
                });
                IMapper mapper = config.CreateMapper();
                User NewStudent = mapper.Map<StudentCreateReqModel, User>(CreateStudent);

                // Tạo mật khẩu ngẫu nhiên nếu không có
                if (CreateStudent.Password == null)
                {
                    CreateStudent.Password = Encoder.GenerateRandomPassword();
                }

                // Lấy nội dung tệp HTML từ Firebase Storage
                string url = "https://firebasestorage.googleapis.com/v0/b/axlm-2024.appspot.com/o/Data%2FUtilities%2FTemplateEmail%2FFirstInformation.html?alt=media&token=YOUR_TOKEN_HERE";
                string Html;

                using (HttpClient httpClient = new HttpClient())
                {
                    // Tải nội dung HTML từ Firebase Storage
                    Html = await httpClient.GetStringAsync(url);
                }

                // Thay thế các biến trong HTML
                Html = Html.Replace("{{Password}}", CreateStudent.Password);
                Html = Html.Replace("{{Email}}", CreateStudent.Email);

                // Gửi email
                bool check = await _email.SendEmail(CreateStudent.Email, "Login Information", Html);
                if (!check)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Failed to send email with login information!";
                    return Result;
                }

                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                var HashedPasswordModel = Encoder.CreateHashPassword(CreateStudent.Password);
                NewStudent.Password = HashedPasswordModel.HashedPassword;
                NewStudent.Salt = HashedPasswordModel.Salt;
                NewStudent.Role = Roles.STUDENT;
                NewStudent.Status = UserStatus.ACTIVE;

                // Lưu sinh viên mới vào cơ sở dữ liệu
                await _studentRepository.Insert(NewStudent);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Create student successfully!";
            }
            catch (Exception e)
            {
                // Ghi log thông tin chi tiết cho mục đích gỡ lỗi
                Console.WriteLine($"Exception occurred: {e.Message}");
                Console.WriteLine($"StackTrace: {e.StackTrace}");

                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }


        public async Task<ResultModel> CreateStudentList(List<StudentCreateReqModel> CreateStudentList)
        {
            ResultModel Result = new();
            try
            {
                // Kiểm tra trùng lặp email trong danh sách sinh viên
                HashSet<string> emailListCheck = new HashSet<string>();
                foreach (var student in CreateStudentList)
                {
                    if (emailListCheck.Contains(student.Email))
                    {
                        return new ResultModel
                        {
                            Code = 400,
                            IsSuccess = false,
                            Message = $"The email '{student.Email}' of student '{student.FullName}' is duplicated. Please change the email before import again."
                        };
                    }
                    emailListCheck.Add(student.Email);
                }

                var emailList = CreateStudentList.Select(s => s.Email).ToList();

                // Lấy danh sách sinh viên đã tồn tại trong cơ sở dữ liệu
                var userList = await _studentRepository.GetStudentListByEmail(emailList);
                if (userList.Any())
                {
                    var existedEmail = userList.Select(u => u.Email).ToList();
                    return new ResultModel
                    {
                        Code = 400,
                        IsSuccess = false,
                        Message = $"The email {string.Join(", ", existedEmail)} already exists."
                    };
                }

                var newStudentList = new List<User>();
                var emailReqList = new List<EmailSendingModel>();
                string url = "https://firebasestorage.googleapis.com/v0/b/axlm-2024.appspot.com/o/Data%2FUtilities%2FTemplateEmail%2FFirstInformation.html?alt=media&token=YOUR_TOKEN_HERE";

                // Tạo danh sách sinh viên mới
                foreach (var createStudent in CreateStudentList)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<StudentCreateReqModel, User>()
                           .ForMember(dest => dest.Password, opt => opt.Ignore());
                    });
                    IMapper mapper = config.CreateMapper();
                    User NewStudent = mapper.Map<StudentCreateReqModel, User>(createStudent);

                    if (createStudent.Password == null)
                    {
                        createStudent.Password = Encoder.GenerateRandomPassword();
                    }

                    // Tải nội dung tệp HTML từ Firebase Storage
                    using (HttpClient httpClient = new HttpClient())
                    {
                        string Html = await httpClient.GetStringAsync(url);
                        Html = Html.Replace("{{Password}}", createStudent.Password);
                        Html = Html.Replace("{{Email}}", createStudent.Email);

                        // Lưu thông tin email để gửi
                        emailReqList.Add(new EmailSendingModel
                        {
                            email = createStudent.Email,
                            html = Html,
                        });
                    }

                    // Mã hóa mật khẩu và thêm sinh viên mới vào danh sách
                    var HashedPasswordModel = Encoder.CreateHashPassword(createStudent.Password);
                    NewStudent.Password = HashedPasswordModel.HashedPassword;
                    NewStudent.Salt = HashedPasswordModel.Salt;
                    NewStudent.Role = Roles.STUDENT;
                    NewStudent.Status = UserStatus.ACTIVE;
                    newStudentList.Add(NewStudent);
                }

                // Gửi email cho danh sách sinh viên mới
                bool emailSent = await _email.SendListEmail("Login Information", emailReqList);
                if (!emailSent)
                {
                    return new ResultModel
                    {
                        Code = 400,
                        IsSuccess = false,
                        Message = "Failed to send emails to the new students."
                    };
                }

                // Lưu danh sách sinh viên mới vào cơ sở dữ liệu
                await _studentRepository.AddRange(newStudentList);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "New student list added successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }


        public async Task<ResultModel> UpdateStudent(StudentUpdateResModel? studentUpdateRequest, Guid studentId)
        {
            ResultModel Result = new();
            try
            {
                if (studentUpdateRequest is null)
                {
                    throw new ArgumentNullException(nameof(studentUpdateRequest));
                }

                var studentUpdate = await _studentRepository.GetStudentById(studentId);

                if (studentUpdate is null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    return Result;
                }

                studentUpdate.Dob = studentUpdateRequest.Dob;
                studentUpdate.Address = studentUpdateRequest.Address;
                studentUpdate.Gender = studentUpdateRequest.Gender;
                studentUpdate.Phone = studentUpdateRequest.Phone;
                studentUpdate.Status = studentUpdateRequest.Status;


                await _studentRepository.Update(studentUpdate);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Update student successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }


            return Result;
        }

        public async Task<ResultModel> UpdateStudentsByStatus(List<StudentUpdateStatusResModel> studentUpdateStatusRequest)
        {
            ResultModel Result = new ResultModel();
            try
            {
                if (studentUpdateStatusRequest == null || !studentUpdateStatusRequest.Any())
                {
                    throw new ArgumentNullException(nameof(studentUpdateStatusRequest), "No student updates provided for status update.");
                }

                var studentsToUpdate = studentUpdateStatusRequest.ToList();

                foreach (var updateRequest in studentsToUpdate)
                {
                    var studentUpdate = await _studentRepository.GetStudentById(updateRequest.Id);

                    if (studentUpdate == null || studentUpdate.Role != "Student")
                    {
                        return new ResultModel
                        {
                            Code = 404,
                            IsSuccess = false,
                            Message = studentUpdate == null ? "Student not found" : "The user ID is not a student!"
                        };
                    }

                    studentUpdate.Status = updateRequest.Status;
                    await _studentRepository.Update(studentUpdate);
                }

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Update student status successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return Result;
        }

        public async Task<ResultModel> GetScoreListByStudentId(Guid studentId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var listScore = await _studentClassRepository.GetScoreListOfStudentId(studentId);
                var student = await _userRepository.GetUserById(studentId);
                var result = new StudentScoreModel
                {
                    Id = studentId,
                    Name = student.FullName,
                    StudentScores = listScore,
                    GPA = listScore.Select(l => l.ModuleAvgScore).Average()
                };

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = result;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;

        }

        public async Task<ResultModel> GetUnassignedStudents(int page, FilterModel reqModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var students = await _studentClassRepository.GetUnassignedStudent(reqModel);
                if (students.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "There is no unassigned student"
                    };
                }


                var results = await Pagination.GetPagination(students, page, 10);
                resultModel.IsSuccess = true;
                resultModel.Data = results;
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

        public async Task<ResultModel> UpdateStudentScoreListInModule(UpdateStudentScoreListReqModel reqModel, Guid studentId, Guid moduleId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                List<Submission> submissionList = await _submissionRepository.GetSubmissionListByStudentAndModule(studentId, moduleId);

                foreach (var submission in submissionList)
                {
                    decimal score = 0;

                    score = reqModel.NewScoreList[submissionList.IndexOf(submission)];

                    submission.Score = score;
                    await _submissionRepository.Update(submission);
                }

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Data = submissionList;
                resultModel.Message = "Update successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetAdminAndTrainerOfStudent(Guid studentId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var result = await _studentClassRepository.GetAdminAndTrainerOfStudent(studentId);

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = result;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }
    }
}