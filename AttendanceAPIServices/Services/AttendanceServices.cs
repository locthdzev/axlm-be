using System.Net;
using Data.Entities;
using Data.Models.AttendanceModel;
using Data.Models.ResultModel;
using Data.Utilities.Pagination;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Repositories.AttendanceRepositories;
using Repositories.AttendDetaillsRepositories;
using Repositories.ClassRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace AttendanceAPIServices.Services
{
    public class AttendanceServices : IAttendanceServices
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendDetaillsRepository _attendDetaillsRepository;
        private readonly IClassRepository _classRepository;
        private readonly IConfiguration _configuration;

        public AttendanceServices(IAttendanceRepository attendanceRepository, IAttendDetaillsRepository attendDetaillsRepository, IClassRepository classRepository, IConfiguration configuration)
        {
            _attendanceRepository = attendanceRepository;
            _attendDetaillsRepository = attendDetaillsRepository;
            _classRepository = classRepository;
            _configuration = configuration;
        }

        public async Task<ResultModel> GetAttendanceById(Guid attendanceId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var attendance = await _attendanceRepository.GetAttendanceById(attendanceId);
                resultModel.Data = attendance;
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

        public async Task<ResultModel> GetAttendanceForStudent(Guid attendanceId, string token)
        {
            ResultModel Result = new();
            try
            {
                Guid userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var attendanceDetail = await _attendDetaillsRepository.GetAttendanceDetailById(attendanceId, userId);
                var attendance = await _attendanceRepository.GetAttendanceById(attendanceId);
                if (attendance == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Attendance not found";
                    return Result;
                }
                var getClass = await _classRepository.GetClassById(attendance.ClassId);
                AttendanceResModel Res = new();
                AttendanceClassModel ClassAttendance = new();
                Res.AttendanceId = attendanceId;
                ClassAttendance.Id = attendance.ClassId;
                ClassAttendance.ClassName = getClass.Name;
                Res.Class = ClassAttendance;
                Res.AttendanceStatus = attendance.Status;
                Res.CreatedAt = attendance.Date;
                Res.IsCheckAttendance = false;
                if (attendanceDetail != null)
                {
                    Res.IsCheckAttendance = true;
                }
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = Res;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> CheckAttendance(Guid attendanceId, string token, CheckAttendanceReqModel req)
        {
            ResultModel Result = new();
            try
            {
                Guid userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var attendance = await _attendanceRepository.GetAttendanceById(attendanceId);
                if (attendance == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Attendance not found";
                    return Result;
                }

                if (!attendance.Code.Equals(req.Code))
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Wrong code";
                    return Result;
                }
                AttendanceDetail attendanceDetail = new();
                attendanceDetail.Id = Guid.NewGuid();
                attendanceDetail.AttendanceId = attendanceId;
                attendanceDetail.StudentId = userId;
                var now = DateTime.Now;
                attendanceDetail.CreatedAt = now;
                attendanceDetail.Status = AttendanceStudentStatus.PRESENT;

                if (attendance.Status.Equals(AttendanceStatus.LATE))
                {
                    attendanceDetail.Status = AttendanceStudentStatus.LATE;
                }

                if (attendance.Status.Equals(AttendanceStatus.DONE))
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The attendance is done";
                    return Result;
                }

                await _attendDetaillsRepository.Insert(attendanceDetail);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Attendance checked successfully";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> CreateAttendance(Guid classId, AttendanceReqModel reqCreate)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var latestAttendance = await _attendanceRepository.GetLatestAttendanceOfClass(classId);

                if (latestAttendance != null &&
                    (latestAttendance.Status.Equals(AttendanceStatus.PROCESSING) || latestAttendance.Status.Equals(AttendanceStatus.LATE)))
                {
                    return new ResultModel
                    {
                        Code = 400,
                        IsSuccess = false,
                        Message = "The latest attendance of this class is not finished"
                    };
                }

                if (reqCreate.AddLateTime == null)
                {
                    reqCreate.AddLateTime = 0;
                }

                var dueTo = DateTime.Now.AddMinutes(reqCreate.AddMinute);
                var newAttendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    ClassId = classId,
                    Date = DateTime.Now,
                    DueTo = dueTo,
                    LateTo = dueTo.AddMinutes(reqCreate.AddLateTime.Value),
                    Status = AttendanceStatus.PROCESSING,
                    Code = new Random().Next(0, 9999).ToString("D4")
                };

                await _attendanceRepository.Insert(newAttendance);

                // SQL Server event scheduling
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    var attendanceId = newAttendance.Id;
                    var eventExpiredName = $"update_expired_attendance_{attendanceId}".Replace("-", "_");
                    var eventLateName = $"update_late_attendance_{attendanceId}".Replace("-", "_");
                    var expiredTime = newAttendance.DueTo;
                    var lateTime = newAttendance.LateTo;
                    var doneStatus = AttendanceStatus.DONE;
                    var lateStatus = AttendanceStatus.LATE;

                    if (lateTime == expiredTime)
                    {
                        string createExpiredEventQuery = $@"
                    CREATE EVENT {eventExpiredName}
                    ON SCHEDULE AT '{expiredTime.ToString("yyyy-MM-dd HH:mm:ss")}'
                    DO
                    BEGIN
                        UPDATE Attendance
                        SET Status = '{doneStatus}'
                        WHERE Id = '{attendanceId}';
                    END;";

                        using (var command = new SqlCommand(createExpiredEventQuery, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    else if (lateTime > expiredTime)
                    {
                        string createExpiredEventQuery = $@"
                    CREATE EVENT {eventExpiredName}
                    ON SCHEDULE AT '{expiredTime.ToString("yyyy-MM-dd HH:mm:ss")}'
                    DO
                    BEGIN
                        UPDATE Attendance
                        SET Status = '{lateStatus}'
                        WHERE Id = '{attendanceId}';
                    END;";

                        using (var command = new SqlCommand(createExpiredEventQuery, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }

                        string createLateEventQuery = $@"
                    CREATE EVENT {eventLateName}
                    ON SCHEDULE AT '{lateTime.ToString("yyyy-MM-dd HH:mm:ss")}'
                    DO
                    BEGIN
                        UPDATE Attendance
                        SET Status = '{doneStatus}'
                        WHERE Id = '{attendanceId}';
                    END;";

                        using (var command = new SqlCommand(createLateEventQuery, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Data = newAttendance;
                resultModel.Message = "Create attendance checking successfully";
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.Message = e.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetAttendanceListOfClass(Guid classId, int page)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var result = await _attendanceRepository.GetListAttendance(classId);
                var finalResult = await Pagination.GetPagination(result, page, 15);

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Data = finalResult;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = e.Message;
            }
            return resultModel;
        }
    }
}