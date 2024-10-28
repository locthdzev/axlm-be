using Data.Entities;
using Data.Enums;
using Data.Models.FilterModel;
using Data.Models.UserModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.UserRepositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AXLMDbContext _context;

        public UserRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string Email)
        {
            return await _context.Users.Where(x => x.Email.Equals(Email) && !x.Status.Equals(UserStatus.INACTIVE)).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(Guid? userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<Guid> GetUserIdByEmail(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            return user.Id;
        }

        public async Task<List<User>> GetAllStudents(FilterModel reqModel)
        {
            return await GetAllStudentsQuery(reqModel).OrderBy(s => s.FullName).ToListAsync();
        }


        private IQueryable<User> GetAllStudentsQuery(FilterModel reqModel)
        {
            var allUser = _context.Users.Where(x => x.Role.Equals("Student"));

            if (reqModel.gender != null)
            {
                allUser = allUser.Where(user => user.Gender.Equals(reqModel.gender, StringComparison.OrdinalIgnoreCase));
            }

            if (reqModel.status != null)
            {
                allUser = allUser.Where(user => user.Status.Equals(reqModel.status, StringComparison.OrdinalIgnoreCase));
            }

            return allUser;
        }

        public async Task<List<User>> GetAllUser(FilterModel reqModel)
        {
            return await GetAllQuery(reqModel).ToListAsync();
        }

        private IQueryable<User> GetAllQuery(FilterModel reqModel)
        {
            var allUser = _context.Users.AsQueryable();

            if (reqModel.gender != null)
            {
                allUser = allUser.Where(user => user.Gender.Equals(reqModel.gender, StringComparison.OrdinalIgnoreCase));
            }

            if (reqModel.role != null)
            {
                allUser = allUser.Where(user => user.Role.Equals(reqModel.role, StringComparison.OrdinalIgnoreCase));
            }

            if (reqModel.status != null)
            {
                allUser = allUser.Where(user => user.Status.Equals(reqModel.status, StringComparison.OrdinalIgnoreCase));
            }

            return allUser;
        }

        public async Task<List<User>> GetStudentsByClassId(Guid classId)
        {
            return await _context.Users
                .Where(u => u.Role.Equals("Student"))
                .Where(u => u.StudentClasses.Any(sc => sc.ClassId == classId))
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<UserResModel> GetAccountInfo(Guid userId)
        {
            return await _context.Users.Where(u => u.Id == userId)
                                       .Select(u => new UserResModel
                                       {
                                           Id = u.Id,
                                           Address = u.Address,
                                           Dob = u.Dob,
                                           Email = u.Email,
                                           FullName = u.FullName,
                                           Gender = u.Gender,
                                           Phone = u.Phone,
                                           Role = u.Role,
                                           Status = u.Status
                                       })
                                       .FirstOrDefaultAsync();
        }

        public async Task<List<UserResModel>> GetTrainerList()
        {
            return await _context.Users.Where(u => u.Role.Equals(Roles.TRAINER))
                .Select(u => new UserResModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Dob = u.Dob,
                    Gender = u.Gender,
                    Address = u.Address,
                    Phone = u.Phone,
                    Role = u.Role,
                    Status = u.Status
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<List<User>> GetUserListByListId(List<Guid> userIdList)
        {
            return await _context.Users.Where(u => userIdList.Contains(u.Id)).ToListAsync();
        }

        public async Task<SenderEmailModel?> GetSenderInfoById(Guid userId)
        {
            return await _context.Users.Where(u => u.Id == userId).Select(u => new SenderEmailModel
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName
            }).FirstOrDefaultAsync();
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

        public async Task<OtpVerify> GetOTPByUserId(Guid UserId)
        {
            return await _context.OtpVerifies.Where(x => x.UserId.Equals(UserId)).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
        }
    }
}