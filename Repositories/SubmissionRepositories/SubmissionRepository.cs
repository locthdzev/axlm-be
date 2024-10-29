using Data.Entities;
using Data.Models.SubmissionModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.SubmissionRepositories
{
    public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
    {
        private readonly AXLMDbContext _context;

        public SubmissionRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Submission> GetSubmissionByStudentAndAssignment(Guid studentId, Guid assignmentId)
        {
            return await _context.Submissions.Where(s => s.StudentId == studentId && s.AssignmentId == assignmentId).FirstOrDefaultAsync();
        }

        public async Task<List<Submission>> GetListSubmissionByAssignmentId(Guid assignmentId)
        {
            return await _context.Submissions.Where(s => s.AssignmentId == assignmentId).ToListAsync();
        }

        public async Task<List<Submission>> GetSubmissionListByStudentId(Guid studentId)
        {
            return await _context.Submissions.Where(s => s.StudentId == studentId).ToListAsync();
        }

        public async Task<List<Submission>> GetSubmissionListOfModuleId(Guid moduleId)
        {
            var assignmentOfModule = await _context.Assignments.Where(a => a.ModuleId == moduleId)
                .Select(a => a.Id)
                .ToListAsync();
            return await _context.Submissions.Where(s => assignmentOfModule.Contains(s.AssignmentId)).ToListAsync();
        }

        public async Task<List<Submission>> GetSubmissionListByStudentAndModule(Guid studentId, Guid moduleId)
        {
            return await _context.Assignments.Where(a => a.ModuleId == moduleId)
                .Join(
                    _context.Submissions,
                    ass => ass.Id,
                    sub => sub.AssignmentId,
                    (ass, sub) => sub
                    )
                .Where(sub => sub.StudentId == studentId)
                .ToListAsync();
        }
        public async Task<List<Submission>> GetAllSubmission()
        {
            return await _context.Submissions.Where(s => s.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<List<Submission>> GetSubmissionByAssignmentId(Guid id)
        {
            return await _context.Submissions.Where(s => s.AssignmentId == id).ToListAsync();
        }

        public async Task<List<ListStudentWithSubmission>> GetListStudentWithSubmissions(Guid assignmentId)
        {
            var classId = await _context.Assignments.Where(a => a.Id == assignmentId).Select(a => a.Class.Id).FirstOrDefaultAsync();
            return await _context.StudentClasses
                .Where(sc => sc.ClassId == classId)
                .GroupJoin(_context.Submissions.Where(s => s.AssignmentId == assignmentId), sc => sc.StudentId, sm => sm.StudentId, (sc, sm) => new { StudentInfo = sc, SubmissionInfo = sm })
                .SelectMany(submission => submission.SubmissionInfo.DefaultIfEmpty(),
                (studentInfo, submission) => new ListStudentWithSubmission
                {
                    AuthorOfSubmission = new AuthorOfSubmission
                    {
                        Id = studentInfo.StudentInfo.StudentId,
                        Name = studentInfo.StudentInfo.Student.FullName,
                    },
                    SubmissionSpecificResModel = submission != null ? new SubmissionSpecificResModel
                    {
                        Id = submission.Id,
                        AssignmentId = submission.AssignmentId,
                        FileName = submission.AttachmentUrl,
                        IsGrade = submission.IsGrade,
                        Score = submission.Score,
                        CreatedAt = submission.CreatedAt,
                        Status = submission.Status
                    } : null
                })
                .ToListAsync();
        }

        public async Task<List<SubmissionCheckModel>> CheckSubmissionOfClass(Guid classId, Guid assignmentId)
        {
            return await _context.StudentClasses.Where(sc => sc.ClassId == classId)
                                        .GroupJoin(_context.Submissions.Where(s => s.AssignmentId == assignmentId)
                                                                , sc => sc.StudentId, s => s.StudentId, (sc, s) => new { Student = sc, Submission = s })
                                        .SelectMany(submission => submission.Submission.DefaultIfEmpty(),
                                        (student, submission) => new SubmissionCheckModel
                                        {
                                            Student = student.Student.Student,
                                            FileName = submission != null ? submission.AttachmentUrl : "-",
                                            isSubmit = submission != null ? true : false,
                                            Score = submission != null ? submission.Score : null,
                                        })
                                        .OrderBy(sc => sc.Student.FullName)
                                        .ToListAsync();
        }
    }
}