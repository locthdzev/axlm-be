using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.AttendDetaillsRepositories
{
    public class AttendDetaillsRepository : Repository<AttendanceDetail>, IAttendDetaillsRepository
    {
        private readonly AXLMDbContext _context;

        public AttendDetaillsRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceDetail?>> GetAllAttendanceDetails()
        {
            return await _context.AttendanceDetails.ToListAsync();
        }

        public async Task<AttendanceDetail?> GetAttendanceDetailById(Guid AttendanceId, Guid UserId)
        {
            return await _context.AttendanceDetails.Where(a => a.AttendanceId == AttendanceId && a.StudentId == UserId).FirstOrDefaultAsync();
        }
    }
}