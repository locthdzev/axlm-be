using Data.Entities;
using Data.Models.AssignmentModel;
using Repositories.GenericRepositories;

namespace Repositories.AssignmentRepositories
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task<Class> GetClassByAssignmentId(Guid assignmentId);
        Task<Assignment> GetAssignmentById(Guid assignmentId);
        Task<List<Assignment>> GetListAssignmentById(List<Guid> assignmentId);
        Task<IEnumerable<Assignment>> GetAssignments();
        Task<Assignment?> GetAssignmentByTitle(string title);
        Task AddAssigmentAsync(Assignment assignment, AssignmentDetail assignmentDetail);
        Task<List<Assignment>> GetAllAssignments();
        Task<List<ModuleAssignmentResModel>> GetModuleListWithAssignmentsOfClass(Guid classId, Guid studentId);
        Task<List<Assignment>> GetListAssignmentByModuleId(Guid moduleId);
    }
}