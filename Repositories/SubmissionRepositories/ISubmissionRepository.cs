using Data.Entities;
using Data.Models.SubmissionModel;
using Repositories.GenericRepositories;

namespace Repositories.SubmissionRepositories
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        Task<Submission> GetSubmissionByStudentAndAssignment(Guid studentId, Guid assignmentId);
        Task<List<Submission>> GetListSubmissionByAssignmentId(Guid assignmentId);
        Task<List<Submission>> GetSubmissionListByStudentId(Guid studentId);
        Task<List<Submission>> GetSubmissionListOfModuleId(Guid moduleId);
        Task<List<Submission>> GetSubmissionListByStudentAndModule(Guid studentId, Guid moduleId);
        Task<List<Submission>> GetAllSubmission();
        Task<List<Submission>> GetSubmissionByAssignmentId(Guid id);
        Task<List<ListStudentWithSubmission>> GetListStudentWithSubmissions(Guid assignmentId);
        Task<List<SubmissionCheckModel>> CheckSubmissionOfClass(Guid classId, Guid assignmentId);
    }
}