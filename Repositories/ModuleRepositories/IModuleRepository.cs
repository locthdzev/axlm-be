using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.ModuleRepositories
{
    public interface IModuleRepository : IRepository<Module>
    {
        Task<IEnumerable<Module>> GetModule();
        Task<Module?> GetModuleById(Guid moduleid);
        Task<Module?> GetModuleByName(string name);
        Task<List<Module>> GetListModuleById(List<Guid> moduleid);
        Task<List<Module>> GetAllModules();
        Task<List<Module>> GetModuleByProgramId(Guid programId);
    }
}