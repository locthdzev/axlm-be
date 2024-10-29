using Data.Entities;
using Data.Models.ModuleModel;
using Data.Models.ModuleProgramModel;
using Repositories.GenericRepositories;

namespace Repositories.ModuleProgramRepositories
{
    public interface IModuleProgramRepository : IRepository<ModuleProgram>
    {
        Task<List<ModuleProgramReqModel>> GetModulesByProgramId(Guid programId);
        Task<List<OtherModuleListRes>> GetModuleOfOtherProgram(Guid programId);
        Task<List<ModuleProgram>> GetModuleProgramByProgramId(Guid programId);
        Task<List<ModuleProgram>> GetModuleProgramByModuleId(Guid moduleId);
    }
}