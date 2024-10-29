using Data.Entities;
using Data.Models.AttendanceModel;
using Repositories.GenericRepositories;

namespace Repositories.AttendanceRepositories
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance?> GetAttendanceById(Guid attendanceId);
        Task<Attendance?> GetLatestAttendanceOfClass(Guid classId);
        Task<List<AttendanceListOfClassModel>> GetListAttendance(Guid classId);
        Task<IEnumerable<Attendance>> GetAllAttendance();
    }
}