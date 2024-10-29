using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.ClassManagerRepositories
{
    public interface IClassManagerRepository : IRepository<ClassManager>
    {
        Task<ClassManager?> GetClassManagerByClassId(Guid classId);
        Task<List<Guid>> GetListClassIdByManagerId(Guid managerId);
    }
}