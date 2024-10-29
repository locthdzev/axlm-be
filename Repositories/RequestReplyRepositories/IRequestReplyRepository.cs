using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.RequestReplyRepositories
{
    public interface IRequestReplyRepository : IRepository<RequestReply>
    {
        Task<List<RequestReply>> GetRepliesByEmailId(Guid emailId);
    }
}