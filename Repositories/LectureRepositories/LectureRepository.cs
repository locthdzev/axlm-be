using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.LectureRepositories
{
    public class LectureRepository : Repository<Lecture>, ILectureRepository
    {
        private readonly AXLMDbContext _context;

        public LectureRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Lecture?> GetLectureById(Guid lectureId)
		{
			return await _context.Lectures
				.Include(l => l.Class)
				.FirstOrDefaultAsync(l => l.Id == lectureId);
		}

		public async Task<Lecture?> GetLectureByName(string lectureName)
		{
			return await _context.Lectures
				.Include(l => l.Class)
				.FirstOrDefaultAsync(l => l.Name == lectureName);
		}

        public async Task<Lecture> UpdateLecture(Lecture lecture)
        {
            var lectureUpdate = _context.Lectures
                .FirstOrDefault(l => l.Id == lecture.Id);
            if (lectureUpdate is null)
            {
                return lecture;
            }
            lectureUpdate.Order = lecture.Order;
            lectureUpdate.ModuleId = lecture.ModuleId;
            lectureUpdate.ClassId = lecture.ClassId;
            lectureUpdate.Name = lecture.Name;
            lectureUpdate.CreatedBy = lecture.CreatedBy;
            lectureUpdate.UpdatedAt = lecture.UpdatedAt;
            lectureUpdate.UpdatedBy = lecture.UpdatedBy;
            lectureUpdate.Status = lecture.Status;

			await _context.SaveChangesAsync();
			return lectureUpdate;
		}

		public async Task<List<Lecture>> GetListLectures()
		{
			return await _context.Lectures.Where(l => l.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
		}

        public async Task<List<Lecture>> GetListLecturesById(List<Guid> lectureId)
        {
            return await _context.Lectures.Where(l => lectureId.Contains(l.Id)).ToListAsync();
        }
        public async Task<List<Lecture>> GetAllLectures()
        {
            return await _context.Lectures.Where(l => l.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

		public async Task<List<Lecture>> GetLectureByModuleId(Guid moduleId)
		{
			return await _context.Lectures
				.Where(l => l.ModuleId == moduleId)
				.OrderBy(l => l.CreatedAt)
				.ToListAsync();
		}
    }
}