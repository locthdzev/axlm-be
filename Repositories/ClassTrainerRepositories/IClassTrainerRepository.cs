using Data.Entities;
using Data.Models.ClassModel;
using Repositories.GenericRepositories;

namespace Repositories.ClassTrainerRepositories
{
    public interface IClassTrainerRepository : IRepository<ClassTrainer>
    {
        Task<ClassTrainer?> GetClassTrainerByClassId(Guid classId);
        Task<List<NoneTrainerClassListModel>> GetNoneTrainerClasses();
        Task<List<Guid>> GetListClassIdByTrainerId(Guid trainerId);
    }
}