using Data.Entities;
using Data.Enums;
using Data.Models.ClassModel;
using Data.Models.FilterModel;
using Data.Models.SubmissionModel;
using Data.Models.UserModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.StudentClassRepositories
{
    public class StudentClassRepository : Repository<StudentClass>, IStudentClassRepository
    {
        private readonly AXLMDbContext _context;

        public StudentClassRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserResModel>> GetUnassignedStudent(FilterModel reqModel)
        {
            return await GetUnassignedStudentFilter(reqModel).OrderBy(u => u.FullName).ToListAsync();
        }

        public IQueryable<UserResModel> GetUnassignedStudentFilter(FilterModel reqModel)
        {
            var assignedStudentIds = _context.StudentClasses.Select(a => a.StudentId);

            var result = _context.Users.Where(u => u.Role.Equals(Roles.STUDENT) && u.Status == UserStatus.ACTIVE
                                          && !assignedStudentIds.Contains(u.Id))
                .Select(u => new UserResModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Address = u.Address,
                    Dob = u.Dob,
                    Gender = u.Gender,
                    Phone = u.Phone,
                    Status = u.Status,
                    Role = u.Role
                });

            if (!String.IsNullOrEmpty(reqModel.gender))
            {
                result = result.Where(user => user.Gender.Equals(reqModel.gender, StringComparison.OrdinalIgnoreCase));
            }

            if (reqModel.searchValue != null)
            {
                result = result.Where(user => user.FullName.Contains(reqModel.searchValue, StringComparison.OrdinalIgnoreCase) ||
                                                user.Email.Contains(reqModel.searchValue, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }

        public async Task<ClassInformationOfStudent> GetClassInfoByStudentId(Guid studentId)
        {
            var classId = await _context.StudentClasses.Where(sc => sc.StudentId == studentId).Select(sc => sc.ClassId)
                .FirstOrDefaultAsync();

            if (classId == Guid.Empty)
            {
                return null;
            }

            var classEntity = await _context.Classes.Where(c => c.Id == classId).FirstOrDefaultAsync();
            var admin = await _context.ClassManagers.Where(cm => cm.ClassId == classId).Select(cm => cm.User).FirstOrDefaultAsync();
            var program = await _context.TrainingPrograms.Where(tp => tp.Id == classEntity.ProgramId).FirstOrDefaultAsync();
            var trainerId = await _context.ClassTrainers.Where(cm => cm.ClassId == classId).Select(cm => cm.UserId).FirstOrDefaultAsync();
            var trainer = await _context.Users.Where(u => u.Id == trainerId).FirstOrDefaultAsync();

            return new ClassInformationOfStudent
            {
                ClassInformationOfOthers = new ClassInformationOfOthers
                {
                    ClassId = classId,
                    ClassName = classEntity.Name,
                    ProgramId = program.Id,
                    Program = program.Name,
                    StartAt = classEntity.StartAt,
                    EndAt = classEntity.EndAt,
                    Location = classEntity.Location,
                    TrainerId = trainerId != Guid.Empty ? trainerId : null,
                    Trainer = trainer != null ? trainer.FullName : null,
                },
                AdminOfCLass = new AdminOfCLass
                {
                    Id = admin.Id,
                    Name = admin.FullName,
                }
            };
        }

        public async Task<List<ClassInformationOfOthers>?> GetClassInfoByOtherId(Guid otherId)
        {
            var role = await _context.Users.Where(u => u.Id == otherId).Select(u => u.Role).FirstOrDefaultAsync();
            var classIdList = new List<Guid>();

            if (role.Equals(Roles.ADMIN) || role.Equals(Roles.SUADMIN))
            {
                classIdList = await _context.ClassManagers.Where(sc => sc.UserId == otherId).Select(sc => sc.ClassId)
                    .ToListAsync();
            }
            else
            {
                classIdList = await _context.ClassTrainers.Where(sc => sc.UserId == otherId).Select(sc => sc.ClassId)
                    .ToListAsync();
            }

            if (classIdList.Count > 0)
            {
                var classEntity = await _context.Classes
                    .Select(c => new
                    {
                        Class = c,
                        Trainers = _context.ClassTrainers.Where(ct => ct.ClassId == c.Id).ToList(),
                    })
                    .Select(cct => new
                    {
                        Class = cct.Class,
                        Trainers = cct.Trainers.FirstOrDefault(),
                        Managers = _context.ClassManagers.FirstOrDefault(cm => cm.ClassId == cct.Class.Id)
                    })
                    .Select(ci => new ClassInformationOfOthers
                    {
                        ClassId = ci.Class.Id,
                        ClassName = ci.Class.Name,
                        ProgramId = ci.Class.ProgramId,
                        Program = ci.Class.Program.Name,
                        StartAt = ci.Class.StartAt,
                        EndAt = ci.Class.EndAt,
                        Location = ci.Class.Location,
                        TrainerId = ci.Trainers != null && ci.Trainers.UserId != Guid.Empty ? ci.Trainers.UserId : null,
                        Trainer = ci.Trainers != null && ci.Trainers.UserId != Guid.Empty ? ci.Trainers.User.FullName : null,
                        AdminId = ci.Managers != null ? ci.Managers.UserId : null,
                        Admin = ci.Managers != null ? ci.Managers.User.FullName : null
                    })
                    .Where(ce => classIdList.Contains(ce.ClassId))
                    .ToListAsync();

                return classEntity;
            }

            return null;
        }

        public int GetNumberOfStudents(Guid classId)
        {
            return _context.StudentClasses.Where(c => c.ClassId.Equals(classId)).ToList().Count;
        }

        public async Task<List<User>> GetStudentClassByClassId(Guid classId)
        {
            var StudentsInClass = await _context.StudentClasses.Where(c => c.ClassId.Equals(classId) && c.Status.Equals(GeneralStatus.ACTIVE)).Select(a => a.StudentId).ToListAsync();
            return await _context.Users.Where(u => StudentsInClass.Contains(u.Id)).ToListAsync();
        }

        public async Task<Class?> GetClassByStudentId(Guid studentId)
        {
            return await _context.Classes.Where(u => u.StudentClasses.Any(s => s.StudentId == studentId)).FirstOrDefaultAsync();
        }

        public async Task<List<UserResModel>> GetStudentsOfClass(Guid classId)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassId == classId)
                .Join(_context.Users,
                    sc => sc.StudentId,
                    s => s.Id,
                    (sc, s) => new UserResModel
                    {
                        Id = s.Id,
                        FullName = s.FullName,
                        Email = s.Email,
                        Dob = s.Dob,
                        Address = s.Address,
                        Gender = s.Gender,
                        Role = s.Role,
                        Phone = s.Phone,
                        Status = s.Status
                    })
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public Task<List<StudentClass>> GetAllStudentsClassByClassId(Guid classId)
        {
            return _context.StudentClasses.Where(sc => sc.ClassId == classId).ToListAsync();
        }

        public async Task<List<ModuleAssignmentModel>> GetScoreListOfStudentId(Guid studentId)
        {
            var classOfStudent = await _context.StudentClasses.Where(sc => sc.StudentId == studentId).Select(sc => sc.ClassId).FirstOrDefaultAsync();
            var trainingProgramId = await _context.Classes.Where(c => c.Id == classOfStudent).Select(c => c.ProgramId).FirstOrDefaultAsync();
            var modules = await _context.Modules.Join(_context.ModulePrograms.Where(mp => mp.ProgramId == trainingProgramId),
                                              m => m.Id,
                                              mp => mp.ModuleId,
                                              (m, mp) => m)
                                       .ToListAsync();

            var assignments = await _context.Assignments.Where(a => modules.Select(m => m.Id).Contains(a.ModuleId)).ToListAsync();
            var submissions = await _context.Submissions.Where(s => assignments.Select(a => a.Id).Contains(s.AssignmentId) && s.StudentId == studentId).ToListAsync();
            var listScore = modules.Select(module =>
            {
                var moduleAssignments = assignments.Where(a => a.ModuleId == module.Id).OrderByDescending(a => a.CreatedAt).ToList();
                decimal? moduleAvgScore = null;

                if (moduleAssignments.Any())
                {
                    moduleAvgScore = moduleAssignments
                    .SelectMany(a => a.Submissions.Where(s => s.IsGrade == true).Select(s => s.Score))
                                                      .DefaultIfEmpty()
                                                      .Average();
                }

                return new ModuleAssignmentModel
                {
                    Id = module.Id,
                    Code = module.Code,
                    Name = module.Name,
                    Assignments = moduleAssignments
                        .Select(a =>
                        {
                            var submission = submissions.Where(s => s.AssignmentId == a.Id).FirstOrDefault();
                            var isScore = submissions.FirstOrDefault(s => s.AssignmentId == a.Id)?.IsGrade;
                            var score = submissions.FirstOrDefault(s => s.AssignmentId == a.Id)?.Score;
                            return new BasicAssignmentModel
                            {
                                Id = a.Id,
                                Title = a.Title,
                                Content = a.Content,
                                CreatedAt = a.CreatedAt,
                                Score = score,
                                Status = (submission != null && isScore == true) ? "Graded" :
                                         (submission != null && isScore == false) ? "Not graded" : "Not submitted"
                            };
                        })
                        .ToList(),
                    ModuleAvgScore = moduleAvgScore
                };
            }).ToList();

            return listScore;
        }

        public async Task<List<ModuleListAvgScore>?> GetModuleListAvgScore(Guid classId)
        {
            var students = await _context.Users.Join(_context.StudentClasses.Where(sc => sc.ClassId == classId), u => u.Id, sc => sc.StudentId, (u, sc) => u).ToListAsync();
            if (students.Count == 0)
            {
                return null;
            }
            var trainingProgramId = await _context.Classes.Where(c => c.Id == classId).Select(c => c.ProgramId).FirstOrDefaultAsync();
            var modules = await _context.Modules.Join(_context.ModulePrograms.Where(mp => mp.ProgramId == trainingProgramId),
                                              m => m.Id,
                                              mp => mp.ModuleId,
                                              (m, mp) => m)
                                       .ToListAsync();
            if (modules.Count == 0)
            {
                return null;
            }
            var assignments = await _context.Assignments.Where(a => modules.Select(m => m.Id).Contains(a.ModuleId)).ToListAsync();
            var submissions = await _context.Submissions.Where(s => assignments.Select(a => a.Id).Contains(s.AssignmentId)).ToListAsync();

            var listModules = modules.Select(module =>
                 new ModuleListAvgScore
                 {
                     ModuleId = module.Id,
                     Code = module.Code,
                     studentListInfos = students.OrderBy(s => s.FullName).Select(student =>
                     {
                         var moduleAssignments = assignments.Where(a => a.ModuleId == module.Id).ToList();
                         decimal? moduleAvgScore = null;

                         if (moduleAssignments.Any())
                         {
                             moduleAvgScore = moduleAssignments.SelectMany(a => a.Submissions.Where(s => s.StudentId == student.Id && s.IsGrade == true).Select(s => s.Score))
                                                               .DefaultIfEmpty()
                                                               .Average();
                         }

                         return new StudentListInfo
                         {
                             StudentId = student.Id,
                             Name = student.FullName,
                             AvgScore = moduleAvgScore
                         };
                     })
                     .ToList()
                 }).ToList();
            return listModules;
        }

        public async Task<List<NameAndEmailModel>> GetAdminAndTrainerOfStudent(Guid studentId)
        {
            var admin = await _context.StudentClasses.Where(sc => sc.StudentId == studentId)
                                .Join(_context.ClassManagers, st => st.ClassId, cm => cm.ClassId, (st, cm) => new { AdminId = cm.UserId })
                                .Join(_context.Users, cm => cm.AdminId, u => u.Id, (at, u) => u)
                                .Select(a => new NameAndEmailModel
                                {
                                    Email = a.Email,
                                    Name = a.FullName,
                                    Role = "Admin"
                                })
                                .FirstOrDefaultAsync();
            var trainer = await _context.StudentClasses.Where(sc => sc.StudentId == studentId)
                                .Join(_context.ClassTrainers, st => st.ClassId, ct => ct.ClassId, (st, ct) => new { TrainerId = ct.UserId })
                                .Join(_context.Users, cm => cm.TrainerId, u => u.Id, (at, u) => u)
                                .Select(a => new NameAndEmailModel
                                {
                                    Email = a.Email,
                                    Name = a.FullName,
                                    Role = "Trainer"
                                })
                                .FirstOrDefaultAsync();
            var result = new List<NameAndEmailModel>();

            if (admin != null)
            {
                result.Add(admin);
            }

            if (trainer != null)
            {
                result.Add(trainer);
            }

            return result;
        }
    }
}