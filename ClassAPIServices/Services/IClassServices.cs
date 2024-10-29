using Data.Models.ClassModel;
using Data.Models.ResultModel;
using Data.Models.StudentClassModel;

namespace ClassAPIServices.Services
{
    public interface IClassServices
    {
        Task<ResultModel> GetClassList(int page, FilterDayModel reqModel);
        Task<ResultModel> GetClassInformation(Guid classId, string token);
        Task<ResultModel> CreateClass(ClassReqModel CreateForm, string token);
        Task<ResultModel> UpdateClass(ClassUpdateReqModel CreateForm, Guid classId);
        Task<ResultModel> DeleteClass(DeleteClassReqModel reqModel);
        Task<ResultModel> GetStudentsByClassId(Guid classId, int page);
        Task<ResultModel> AddStudentToClass(AddStudentToClassReqModel reqModel, Guid classId);
        Task<ResultModel> AddTrainerToClass(AddTrainerModel reqModel, Guid classId);
        Task<ResultModel> GetClassOfStudent(Guid userId);
        Task<ResultModel> DeleteStudentsFromClass(Guid ClassId, List<Guid> StudentsId);
        Task<ResultModel> GetListModuleLectureByClassId(Guid classId);
        Task<ResultModel> GetClassesOfManagerAndTrainer(string token);
        Task<ResultModel> GetNoneTrainerClassList();
        Task<ResultModel> GetListModuleLecture(string token);
        Task<ResultModel> GetModuleListAvgScoreOfClass(Guid classId);
    }
}