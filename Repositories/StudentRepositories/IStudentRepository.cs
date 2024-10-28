using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.StudentRepositories
{
    public interface IStudentRepository : IRepository<User>
    {
        Task<User?> GetStudentByEmail(string email);
        Task<User?> GetStudentById(Guid id);
        Task<User?> UpdateStudent(User student);
        Task<List<User>> GetStudentListByEmail(List<string> emailList);
        Task<List<User>> GetStudents();
    }
}