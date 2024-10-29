using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.AttendDetaillsRepositories
{
    public interface IAttendDetaillsRepository : IRepository<AttendanceDetail>
    {
        public Task<AttendanceDetail?> GetAttendanceDetailById(Guid AttendanceDetailId, Guid UserId);
        public Task<IEnumerable<AttendanceDetail?>> GetAllAttendanceDetails();
    }
}