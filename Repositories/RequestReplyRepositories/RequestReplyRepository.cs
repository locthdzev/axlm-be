using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.RequestReplyRepositories
{
    public class RequestReplyRepository : Repository<RequestReply>, IRequestReplyRepository
    {
        private readonly AXLMDbContext _context;

        public RequestReplyRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RequestReply>> GetRepliesByEmailId(Guid emailId)
        {
            return await _context.RequestReplies.Where(x => x.RequestId == emailId).ToListAsync();
        }
    }
}