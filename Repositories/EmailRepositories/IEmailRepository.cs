using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.EmailRepositories
{
    public interface IEmailRepository : IRepository<EmailRequest>
    {
        Task<List<EmailRequest>> GetEmailRequestsByUserId(Guid userId);
        Task<IEnumerable<RequestReply>> GetReplyEmailByRequestId(Guid requestId);
        Task<List<EmailRequest>> GetEmailReceivedByUserId(Guid userId);
        Task<EmailRequest?> GetEmailById(Guid emailId);
        Task<List<EmailRequest>> GetEmailList(Guid userId);
    }
}