using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.StudentRepositories
{
    public class StudentRepository : Repository<User>, IStudentRepository
    {
        private readonly AXLMDbContext _context;

        public StudentRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetStudentByEmail(string Email)
        {
            return await context.Users.Where(x => x.Email.Equals(Email)).FirstOrDefaultAsync();
        }

        public async Task<User?> GetStudentById(Guid id)
        {
            var matchingUser = await _context.Users.FirstOrDefaultAsync(student => student.Id == id);
            return matchingUser;
        }
        public async Task<User?> UpdateStudent(User student)
        {
            var userUpdate = _context.Users
                .FirstOrDefault(u => u.Id == student.Id);
            if (userUpdate is null)
            {
                return student;
            }
            userUpdate.Dob = student.Dob;
            userUpdate.Address = student.Address;
            userUpdate.Gender = student.Gender;
            userUpdate.Phone = student.Phone;
            userUpdate.Status = student.Status;

            await _context.SaveChangesAsync();
            return userUpdate;
        }
        public async Task<List<User>> GetStudentListByEmail(List<string> emailList)
        {
            return await _context.Users.Where(u => emailList.Contains(u.Email)).ToListAsync();
        }

        public async Task<List<User>> GetStudents()
        {
            return await _context.Users.Where(u => u.Role == "Student").ToListAsync();
        }
    }
}