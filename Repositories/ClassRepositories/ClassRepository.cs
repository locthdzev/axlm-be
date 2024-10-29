using Data.Entities;
using Data.Models.ClassModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.ClassRepositories
{
    public class ClassRepository : Repository<Class>, IClassRepository
    {
        private readonly AXLMDbContext _context;

        public ClassRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Class>> GetClasses(FilterDayModel reqModel)
        {
            return await GetClassesFilter(reqModel).ToListAsync();
        }

        private IQueryable<Class> GetClassesFilter(FilterDayModel reqModel)
        {
            var allClass = _context.Classes.Where(c => c.Status == ClassStatus.ACTIVE);

            if (reqModel.StartAt != null)
            {
                allClass = allClass.Where(c => c.StartAt >= reqModel.StartAt);
            }

            if (reqModel.EndAt != null)
            {
                allClass = allClass.Where(c => c.EndAt <= reqModel.EndAt);
            }

            if (reqModel.searchValue != null)
            {
                allClass = allClass.Where(c => c.Name.Contains(reqModel.searchValue, StringComparison.OrdinalIgnoreCase));
            }

            return allClass;
        }

        public async Task<Class?> GetClassById(Guid classId)
        {
            return await _context.Classes
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.Id == classId);
        }

        public async Task<List<Class>> GetListClassById(List<Guid> classId)
        {
            return await _context.Classes.Where(ce => classId.Contains(ce.Id)).ToListAsync();
        }

        public async Task<Class> GetClassByUserId(Guid userId)
        {
            return await _context.Classes
                .Include(c => c.StudentClasses)
                .FirstOrDefaultAsync(c => c.StudentClasses.Any(sc => sc.StudentId == userId));
        }

        public async Task<Class?> GetClassByName(string className)
        {

            return await _context.Classes
                    .Include(c => c.Assignments)
                    .FirstOrDefaultAsync(c => c.Name == className && c.Status.Equals(ClassStatus.ACTIVE));
        }

        public async Task<List<Class>> GetListClassByTrainingProgramId(Guid TrainingProgramId)
        {
            return await _context.Classes.Where(c => c.ProgramId == TrainingProgramId && c.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<List<ClassResModelDisplayInProgram>> GetClassesByTrainingProgramId(Guid TrainingProgramId)
        {
            return await _context.Classes.Where(c => c.ProgramId == TrainingProgramId && c.Status.Equals(GeneralStatus.ACTIVE))
                .Include(c => c.StudentClasses)
                .Select(c => new ClassResModelDisplayInProgram
                {
                    id = c.Id,
                    Name = c.Name,
                    numberOfStudent = c.StudentClasses.Count,
                })
                .ToListAsync();
        }

        public async Task<List<Guid>> GetAllClassIdList()
        {
            return await _context.Classes.Select(c => c.Id).ToListAsync();
        }

        public async Task<List<ModuleLectureResModel>> GetModuleLecturesByClassId(Guid ClassId)
        {
            var Class = await _context.Classes.Where(c => c.Id.Equals(ClassId)).FirstOrDefaultAsync();
            var ModuleData = await _context.ModulePrograms.Where(mp => mp.ProgramId.Equals(Class.ProgramId))
                .Join(
                    _context.Modules,
                    mp => mp.ModuleId,
                    m => m.Id,
                    (mp,
                        m) => m
                ).Join(
                    _context.Users,
                    m => m.CreatedBy,
                    u => u.Id,
                    (m, u) => new { Module = m, User = u }
                )
                .ToListAsync();
            var LectureData = await _context.Lectures
                .Where(l => ModuleData.Select(m => m.Module.Id).Contains(l.ModuleId)).Join(
                _context.Users,
                l => l.CreatedBy,
                u => u.Id,
                (l, u) => new { Lecture = l, User = u }
            ).ToListAsync();
            var DocumentData = await _context.Documents.Where(d => LectureData.Select(l => l.Lecture.Id).Contains(d.LectureId)).Join(
                _context.Users,
                d => d.CreatedBy,
                u => u.Id,
                (d, u) => new { Document = d, User = u }
            ).ToListAsync();
            var resultList = ModuleData.Select(m =>
                new ModuleLectureResModel()
                {
                    Id = m.Module.Id,
                    Name = m.Module.Name,
                    Code = m.Module.Code,
                    CreatedAt = m.Module.CreatedAt,
                    CreatedBy = new AuthorModuleResModel()
                    {
                        Id = m.User.Id,
                        Name = m.User.FullName
                    },
                    Status = m.Module.Status,
                    Lectures = LectureData
                    .OrderBy(l => l.Lecture.CreatedAt)
                    .Where(l => l.Lecture.ModuleId.Equals(m.Module.Id)).Select(l => new LectureOfModuleResModel()
                    {
                        Id = l.Lecture.Id,
                        Name = l.Lecture.Name,
                        CreatedAt = l.Lecture.CreatedAt,
                        CreatedBy = new AuthorModuleResModel()
                        {
                            Id = l.User.Id,
                            Name = l.User.FullName
                        },
                        Documents = DocumentData.Where(d => d.Document.LectureId.Equals(l.Lecture.Id)).Select(d => new LectureDocumentModel()
                        {
                            Id = d.Document.Id,
                            FileName = d.Document.FileName,
                            CreatedAt = d.Document.CreatedAt,
                            CreatedBy = new AuthorModuleResModel()
                            {
                                Id = d.User.Id,
                                Name = d.User.FullName
                            }
                        }).ToList()
                    }).ToList()
                }).ToList();
            return resultList;
        }
    }
}