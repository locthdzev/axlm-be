using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.OtpRepositories
{
    public class OtpRepository : Repository<OtpVerify>, IOtpRepository
    {
        private readonly AXLMDbContext _context;

        public OtpRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OtpVerify> GetOTPByUserId(Guid UserId)
        {
            return await _context.OtpVerifies.Where(x => x.UserId.Equals(UserId)).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
        }
    }
}