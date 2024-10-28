using Data.Models.FilterModel;
using Data.Models.ResultModel;
using Data.Models.StudentModel;

namespace StudentAPIServices.Services
{
    public interface IStudentServices
    {
        Task<ResultModel> GetAllStudents(int page, FilterModel reqModel);
        Task<ResultModel> CreateStudent(StudentCreateReqModel CreateStudent);
        Task<ResultModel> CreateStudentList(List<StudentCreateReqModel> CreateStudentList);
        Task<ResultModel> UpdateStudent(StudentUpdateResModel UpdateStudent, Guid studentId);
        Task<ResultModel> UpdateStudentsByStatus(List<StudentUpdateStatusResModel> UpdateStudentByStatus);
        Task<ResultModel> GetScoreListByStudentId(Guid studentId);
        Task<ResultModel> GetUnassignedStudents(int page,FilterModel reqModel);
        Task<ResultModel> UpdateStudentScoreListInModule(UpdateStudentScoreListReqModel reqModel, Guid studentId, Guid moduleId);
        Task<ResultModel> GetAdminAndTrainerOfStudent(Guid studentId);
    }
}