using Data.Entities;
using Data.Models.FilterModel;
using Repositories.GenericRepositories;
using Data.Models.UserModel;

namespace Repositories.UserRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmail(string Email);
        Task<Guid> GetUserIdByEmail(string email);
        Task<List<User>> GetAllStudents(FilterModel reqModel);
        Task<List<User>> GetAllUser(FilterModel reqModel);
        Task<List<User>> GetStudentsByClassId(Guid classId);
        Task<User> GetUserById(Guid? userId);
        Task<UserResModel> GetAccountInfo(Guid userId);
        Task<List<UserResModel>> GetTrainerList();
        Task<List<User>> GetUserListByListId(List<Guid> userIdList);
        Task<SenderEmailModel?> GetSenderInfoById(Guid userId);
        Task<User?> GetStudentByEmail(string email);
        Task<User?> GetStudentById(Guid id);
        Task<User?> UpdateStudent(User student);
        Task<List<User>> GetStudentListByEmail(List<string> emailList);
        Task<List<User>> GetStudents();
        Task<OtpVerify> GetOTPByUserId(Guid UserId);
    }
}