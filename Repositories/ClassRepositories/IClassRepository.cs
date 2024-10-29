using Data.Entities;
using Data.Models.ClassModel;
using Data.Models.ModuleModel;
using Data.Models.TrainingProgramModel;
using Repositories.GenericRepositories;

namespace Repositories.ClassRepositories
{
    public interface IClassRepository : IRepository<Class>
    {
        Task<List<Class>> GetClasses(FilterDayModel reqModel);
        Task<Class?> GetClassById(Guid classId);
        Task<List<Class>> GetListClassById(List<Guid> classId);
        Task<Class?> GetClassByName(string className);
        Task<List<Class>> GetListClassByTrainingProgramId(Guid TrainingProgramId);
        Task<List<ClassResModelDisplayInProgram>> GetClassesByTrainingProgramId(Guid TrainingProgramId);
        Task<Class> GetClassByUserId(Guid userId);
        Task<List<Guid>> GetAllClassIdList();
        Task<List<ModuleLectureResModel>> GetModuleLecturesByClassId(Guid ClassId);
    }
}