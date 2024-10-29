using Data.Entities;
using Data.Models.TrainingProgramModel;
using Repositories.GenericRepositories;

namespace Repositories.TrainingProgramRepositories
{
    public interface ITrainingProgramRepository : IRepository<TrainingProgram>
    {
        Task<TrainingProgram?> GetTrainingProgramById(Guid TrainingProgramId);
        Task<TrainingProgram?> GetTrainingProgramByName(string TrainingProgramName);
        Task<List<TrainingProgram>> GetListProgramTraining();
        Task<List<TrainingProgramDropDownResModel>> GetListDropDownProgramTraining();
        Task<List<TrainingProgram>> GetListProgramTrainingByListId(List<Guid> ListId);
    }
}