using Data.Models.ModuleModel;
using Data.Models.ResultModel;

namespace ModuleAPIServices.Services
{
    public interface IModuleServices
    {
        Task<ResultModel> GetListModule(int page);
        Task<ResultModel> GetDetailModule(Guid id);
        Task<ResultModel> CreateModule(ModuleReqModel createform);
        Task<ResultModel> UpdateModule(ModuleUpdateModel UpdateModule, Guid moduleId, string token);
        Task<ResultModel> DeleteModule(List<Guid> ListId);
        Task<ResultModel> ViewStudentResultRecord(Guid moduleId, Guid classId);
        Task<ResultModel> AddModuleByCopy(Guid programId, List<Guid> moduleIdList, string token);
    }
}