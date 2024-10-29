using Data.Entities;
using Data.Models.ClassModel;
using Data.Models.FilterModel;
using Data.Models.SubmissionModel;
using Data.Models.UserModel;
using Repositories.GenericRepositories;

namespace Repositories.StudentClassRepositories
{
    public interface IStudentClassRepository : IRepository<StudentClass>
    {
        Task<List<UserResModel>> GetUnassignedStudent(FilterModel reqModel);
        int GetNumberOfStudents(Guid classId);
        Task<List<User>> GetStudentClassByClassId(Guid classId);
        Task<ClassInformationOfStudent> GetClassInfoByStudentId(Guid studentId);
        Task<List<ClassInformationOfOthers>?> GetClassInfoByOtherId(Guid otherId);
        Task<Class?> GetClassByStudentId(Guid studentId);
        Task<List<UserResModel>> GetStudentsOfClass(Guid classId);
        Task<List<StudentClass>> GetAllStudentsClassByClassId(Guid classId);
        Task<List<ModuleAssignmentModel>> GetScoreListOfStudentId(Guid studentId);
        Task<List<ModuleListAvgScore>?> GetModuleListAvgScore(Guid classId);
        Task<List<NameAndEmailModel>> GetAdminAndTrainerOfStudent(Guid studentId);
    }
}