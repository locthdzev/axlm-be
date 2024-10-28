using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.OtpRepositories
{
    public interface IOtpRepository : IRepository<OtpVerify>
    {
        Task<OtpVerify> GetOTPByUserId(Guid UserId);
    }
}