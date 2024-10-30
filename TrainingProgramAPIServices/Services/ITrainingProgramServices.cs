using Data.Models.ModuleModel;
using Data.Models.ResultModel;
using Data.Models.TrainingProgramModel;

namespace TrainingProgramAPIServices.Services
{
    public interface ITrainingProgramServices
    {
        Task<ResultModel> GetListProgramTraining();
        Task<ResultModel> GetProgramTraining(Guid Id);
        Task<ResultModel> GetListDropDownProgramTraining();
        Task<ResultModel> DeleteTrainingProgram(List<Guid> ListId);
        Task<ResultModel> CreateTrainingProgram(TrainingProgramReqModel createForm);
        Task<ResultModel> UpdateTrainingProgram(TrainingProgramResModel UpdateTrainingProgram, Guid programId);
        Task<ResultModel> GetModuleByProgramId(Guid programId, int page);
        Task<ResultModel> GetModuleOfOthers(Guid programId, int page);
        Task<ResultModel> CreateAndAddModuleToTP(string token, Guid programId, List<ModuleReqCreateAndAddToTP> ListModuleInfo);
        Task<ResultModel> DeleteModuleFromTP(string token, Guid programId, List<Guid> ListModuleId);
    }
}