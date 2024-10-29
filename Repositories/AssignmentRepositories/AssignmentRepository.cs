using Data.Entities;
using Data.Models.AssignmentModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.AssignmentRepositories
{
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {
        private readonly AXLMDbContext _context;

        public AssignmentRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Class> GetClassByAssignmentId(Guid assignmentId)
        {
            var classId = await _context.Assignments.Where(a => a.Id == assignmentId)
                              .Select(a => a.ClassId).FirstOrDefaultAsync();

            return await _context.Classes.Where(c => c.Id == classId).FirstOrDefaultAsync();
        }

        public async Task<Assignment> GetAssignmentById(Guid assignmentId)
        {
            return await _context.Assignments.Where(a => a.Id == assignmentId).FirstOrDefaultAsync();
        }

        public async Task<List<Assignment>> GetListAssignmentById(List<Guid> assignmentId)
        {
            return await _context.Assignments.Where(a => assignmentId.Contains(a.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAssignments()
        {
            return await _context.Assignments.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<Assignment?> GetAssignmentByTitle(string title)
        {
            return await _context.Assignments.FirstOrDefaultAsync(a => a.Title == title);
        }

        public async Task AddAssigmentAsync(Assignment assignment, AssignmentDetail assignmentDetail)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    await context.Assignments.AddAsync(assignment);
                    await context.SaveChangesAsync();

                    assignmentDetail.AssignmentId = assignment.Id;
                    await context.AssignmentDetails.AddAsync(assignmentDetail);
                    await context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<List<Assignment>> GetAllAssignments()
        {
            return await _context.Assignments.Where(x => x.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<List<ModuleAssignmentResModel>> GetModuleListWithAssignmentsOfClass(Guid classId, Guid studentId)
        {
            var trainingProgramId = await _context.Classes.Where(c => c.Id == classId).Select(c => c.ProgramId).FirstOrDefaultAsync();
            var modules = await _context.Modules.Join(_context.ModulePrograms.Where(mp => mp.ProgramId == trainingProgramId),
                                              m => m.Id,
                                              mp => mp.ModuleId,
                                              (m, mp) => m)
                                       .Join(_context.Users,
                                              m => m.CreatedBy,
                                              u => u.Id,
                                              (m, u) => new { ModuleInfo = m, UserInfo = u })
                                       .ToListAsync();
            var assignments = await _context.Assignments.Where(a => modules.Select(m => m.ModuleInfo.Id).Contains(a.ModuleId))
                                               .Join(_context.Users,
                                                      a => a.CreatedBy,
                                                      u => u.Id,
                                                      (a, u) => new { AssignmentInfo = a, UserInfo = u })
                                               .ToListAsync();
            var assignmentDetails = await _context.AssignmentDetails.Where(ad => assignments.Select(a => a.AssignmentInfo.Id).Contains(ad.AssignmentId)).ToListAsync();
            var submissions = await _context.Submissions.Where(s => assignments.Select(a => a.AssignmentInfo.Id).Contains(s.AssignmentId)).ToListAsync();

            var studentUse = false;
            if (studentId != Guid.Empty)
            {
                studentUse = true;
            }

            var result = modules.Select(module => new ModuleAssignmentResModel
            {
                Id = module.ModuleInfo.Id,
                Code = module.ModuleInfo.Code,
                Name = module.ModuleInfo.Name,
                CreatedBy = new AuthorAssignmentResModel
                {
                    Id = module.ModuleInfo.CreatedBy,
                    Name = module.UserInfo.FullName,
                },
                CreatedAt = module.ModuleInfo.CreatedAt,
                Assignments = assignments.Where(a => a.AssignmentInfo.ModuleId == module.ModuleInfo.Id)
                                         .Select(a => new AssignmentOfModuleResModel
                                         {
                                             Id = a.AssignmentInfo.Id,
                                             Title = a.AssignmentInfo.Title,
                                             Content = a.AssignmentInfo.Content,
                                             ExpiryDate = a.AssignmentInfo.ExpiryDate,
                                             IsOverTime = a.AssignmentInfo.IsOverTime,
                                             Details = assignmentDetails.Where(ad => ad.AssignmentId == a.AssignmentInfo.Id)
                                             .Select(ad => new AssignmentDetailsResModel
                                             {
                                                 Id = ad.Id,
                                                 FileName = ad.AttachmentUrl,
                                                 CreatedAt = ad.CreatedAt,
                                                 Status = ad.Status,
                                             })
                                             .ToList(),
                                             CreatedAt = a.AssignmentInfo.CreatedAt,
                                             CreatedBy = new AuthorAssignmentResModel
                                             {
                                                 Id = a.AssignmentInfo.CreatedBy,
                                                 Name = a.UserInfo.FullName,
                                             },
                                             NumberOfSubmissions = !studentUse ? submissions.Where(s => s.AssignmentId == a.AssignmentInfo.Id && s.StudentId == studentId).Count() : null,
                                             SubmissionStatus = studentUse ? submissions.Where(s => s.AssignmentId == a.AssignmentInfo.Id && s.StudentId == studentId).FirstOrDefault() != null ? "Submitted" : "Not yet" : null,
                                         }).ToList(),
                Status = module.ModuleInfo.Status,
            }).ToList();

            return result;
        }

        public async Task<List<Assignment>> GetListAssignmentByModuleId(Guid moduleId)
        {
            return await _context.Assignments.Where(a => a.ModuleId == moduleId).ToListAsync(); 
        }
    }
}