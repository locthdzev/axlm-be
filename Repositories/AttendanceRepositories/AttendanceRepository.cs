using Data.Entities;
using Data.Models.AttendanceModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.AttendanceRepositories
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        private readonly AXLMDbContext _context;

        public AttendanceRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendance()
        {
            return await _context.Attendances.ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceById(Guid attendanceId)
        {
            return await _context.Attendances.Where(a => a.Id == attendanceId).FirstOrDefaultAsync();
        }

        public async Task<Attendance?> GetLatestAttendanceOfClass(Guid classId)
        {
            return await _context.Attendances.Where(a => a.ClassId == classId).OrderByDescending(a => a.Date).FirstOrDefaultAsync();
        }

        public async Task<List<AttendanceListOfClassModel>> GetListAttendance(Guid classId)
        {
            var numberOfStudents = await _context.StudentClasses.CountAsync(sc => sc.ClassId == classId);

            return await _context.Attendances.Where(a => a.ClassId == classId).Select(a => new AttendanceListOfClassModel
            {
                AttendanceId = a.Id,
                Date = a.Date,
                numberOfStudents = numberOfStudents,
                numberOfPresentStudent = a.AttendanceDetails.Count(),
                Status = a.Status
            }).ToListAsync();
        }
    }
}