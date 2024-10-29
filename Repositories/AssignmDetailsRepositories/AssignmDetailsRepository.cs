using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.AssignmDetailsRepositories
{
    public class AssignmDetailsRepository : Repository<AssignmentDetail>, IAssignmDetailsRepository
    {
        private readonly AXLMDbContext _context;

        public AssignmDetailsRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AssignmentDetail>> GetAllAttachmentByAssignmentId(Guid AssignmentId)
        {
            return await _context.AssignmentDetails.Where(x => x.AssignmentId.Equals(AssignmentId) && x.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<List<AssignmentDetail>> GetAllAssignmentDetails()
        {
            return await _context.AssignmentDetails.Where(x => x.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }
    }
}