using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.AssignmDetailsRepositories
{
    public interface IAssignmDetailsRepository : IRepository<AssignmentDetail>
    {
        Task<List<AssignmentDetail>> GetAllAttachmentByAssignmentId(Guid AssignmentId);
        Task<List<AssignmentDetail>> GetAllAssignmentDetails();
    }
}