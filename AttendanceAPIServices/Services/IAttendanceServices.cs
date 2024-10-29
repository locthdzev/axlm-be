using Data.Models.AttendanceModel;
using Data.Models.ResultModel;

namespace AttendanceAPIServices.Services
{
    public interface IAttendanceServices
    {
        Task<ResultModel> GetAttendanceById(Guid attendanceId);
        Task<ResultModel> GetAttendanceForStudent(Guid attendanceId, string token);
        Task<ResultModel> CheckAttendance(Guid attendanceId, string token, CheckAttendanceReqModel req);
        Task<ResultModel> CreateAttendance(Guid classId, AttendanceReqModel reqCreate);
        Task<ResultModel> GetAttendanceListOfClass(Guid classId, int page);
    }
}