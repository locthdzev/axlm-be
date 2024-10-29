using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.EmailRepositories
{
    public class EmailRepository : Repository<EmailRequest>, IEmailRepository
    {
        private readonly AXLMDbContext _context;

        public EmailRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<EmailRequest>> GetEmailRequestsByUserId(Guid userId)
        {
           return await _context.EmailRequests
                .Where(e => e.CreatedBy == userId).OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestReply>> GetReplyEmailByRequestId(Guid requestId)
        {
            return await _context.RequestReplies.Where(e => e.RequestId == requestId).OrderBy(e => e.CreatedAt).ToListAsync();
        }

        public async Task<List<EmailRequest>> GetEmailReceivedByUserId(Guid userId)
        {
            return await _context.EmailRequests
                 .Where(e => e.RecipientId == userId)
                 .ToListAsync();
        }

        public async Task<EmailRequest?> GetEmailById(Guid emailId)
        {
            return await _context.EmailRequests.Where(x => x.Id == emailId).FirstOrDefaultAsync();
        }

        public async Task<List<EmailRequest>> GetEmailList(Guid userId)
        {
            return await _context.EmailRequests.Include(e => e.RequestReplies).Where(x => x.RecipientId == userId || x.CreatedBy == userId).ToListAsync();
        }
    }
}