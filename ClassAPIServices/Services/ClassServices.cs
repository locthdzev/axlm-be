using System.Net;
using AutoMapper;
using Data.Entities;
using Data.Enums;
using Data.Models.ClassModel;
using Data.Models.FilterModel;
using Data.Models.ModuleModel;
using Data.Models.ResultModel;
using Data.Models.StudentClassModel;
using Data.Models.UserModel;
using Data.Utilities.Pagination;
using Repositories.ClassManagerRepositories;
using Repositories.ClassRepositories;
using Repositories.ClassTrainerRepositories;
using Repositories.DocumentRepositories;
using Repositories.LectureRepositories;
using Repositories.ModuleRepositories;
using Repositories.StudentClassRepositories;
using Repositories.TrainingProgramRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace ClassAPIServices.Services
{
    public class ClassServices : IClassServices
    {
        private readonly IClassRepository _classRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILectureRepository _lectureRepository;
        private readonly ITrainingProgramRepository _trainingProgramRepository;
        private readonly IClassManagerRepository _classManagerRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IClassTrainerRepository _classTrainerRepository;

        public ClassServices(IClassRepository classRepository, IStudentClassRepository studentClassRepository, IModuleRepository moduleRepository, IUserRepository userRepository, ILectureRepository lectureRepository, ITrainingProgramRepository trainingProgramRepository, IClassManagerRepository classManagerRepository, IDocumentRepository documentRepository, IClassTrainerRepository classTrainerRepository)
        {
            _classRepository = classRepository;
            _studentClassRepository = studentClassRepository;
            _moduleRepository = moduleRepository;
            _userRepository = userRepository;
            _lectureRepository = lectureRepository;
            _trainingProgramRepository = trainingProgramRepository;
            _classManagerRepository = classManagerRepository;
            _documentRepository = documentRepository;
            _classTrainerRepository = classTrainerRepository;
        }

        public async Task<ResultModel> GetClassList(int page, FilterDayModel reqModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var classes = await _classRepository.GetClasses(reqModel);
                List<ClassListResModel> classList = new();
                foreach (var c in classes)
                {
                    var Creator = await _userRepository.GetUserById(c.CreatedBy);
                    CreatorClassModel CreatedBy = new()
                    {
                        Id = Creator.Id,
                        Name = Creator.FullName,
                    };
                    ClassListResModel cl = new()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StartAt = c.StartAt,
                        EndAt = c.EndAt,
                        CreatedAt = c.CreatedAt,
                        CreatedBy = CreatedBy,
                        Location = c.Location,
                        Status = c.Status
                    };
                    classList.Add(cl);

                }

                var ResultList = await Pagination.GetPagination(classList, page, 10);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = ResultList;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetClassInformation(Guid classId, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var user = await _userRepository.GetUserById(userId);
                var validClassList = new List<Guid>();
                if (user.Role.Equals(Roles.ADMIN))
                {
                    validClassList = await _classManagerRepository.GetListClassIdByManagerId(userId);

                }
                else if (user.Role.Equals(Roles.TRAINER))
                {
                    validClassList = await _classTrainerRepository.GetListClassIdByTrainerId(userId);
                }
                else
                {
                    validClassList = await _classRepository.GetAllClassIdList();
                }

                var Class = await _classRepository.GetClassById(classId);
                if (Class == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Class not found";
                    return result;
                }
                var Program = await _trainingProgramRepository.GetTrainingProgramById(Class.ProgramId);
                var CreatedClassByUser = await _userRepository.GetUserById(Class.CreatedBy);
                var CreatedProgramByUser = await _userRepository.GetUserById(Program.CreatedBy);
                var Students = await _studentClassRepository.GetStudentClassByClassId(Class.Id);
                var ListResStudents = new List<UserResModel>();

                foreach (var s in Students)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<User, UserResModel>(); ;
                    });
                    IMapper mapper = config.CreateMapper();
                    UserResModel NewUser = mapper.Map<User, UserResModel>(s);
                    ListResStudents.Add(NewUser);
                }

                CreatorClassModel UpdatorProgramBy = new();
                if (Program.UpdatedBy != null)
                {
                    var UpdatedProgramByUser = await _userRepository.GetUserById(Program.UpdatedBy);
                    UpdatorProgramBy.Id = UpdatedProgramByUser.Id;
                    UpdatorProgramBy.Name = UpdatedProgramByUser.FullName;
                }

                CreatorClassModel UpdatorClassBy = new();
                if (Class.UpdatedBy != null)
                {
                    var UpdatedClassByUser = await _userRepository.GetUserById(Class.UpdatedBy);
                    UpdatorClassBy.Id = UpdatedClassByUser.Id;
                    UpdatorClassBy.Name = UpdatedClassByUser.FullName;
                }

                CreatorClassModel CreatorProgramBy = new()
                {
                    Id = CreatedProgramByUser.Id,
                    Name = CreatedProgramByUser.FullName
                };

                CreatorClassModel CreatorClassBy = new()
                {
                    Id = CreatedClassByUser.Id,
                    Name = CreatedClassByUser.FullName
                };

                ClassProgramDetail classProgramDetail = new()
                {
                    Id = Program.Id,
                    Name = Program.Name,
                    Code = Program.Code,
                    Duration = Program.Duration,
                    CreatedAt = Program.CreatedAt,
                    CreatedBy = CreatorProgramBy,
                    UpdatedAt = Program.UpdatedAt,
                    UpdatedBy = UpdatorProgramBy,
                    Status = Program.Status
                };

                var userList = new List<User>();

                var classManage = await _classManagerRepository.GetClassManagerByClassId(Class.Id);
                if (classManage != null)
                {
                    var admin = await _userRepository.GetUserById(classManage.UserId);
                    userList.Add(admin);
                }

                var classTrain = await _classTrainerRepository.GetClassTrainerByClassId(Class.Id);
                if (classTrain != null)
                {
                    var trainer = await _userRepository.GetUserById(classTrain.UserId);
                    userList.Add(trainer);
                }

                var trainerManagerList = new List<ClassManagerAndTrainer>();
                if (userList.Count > 0)
                {
                    foreach (var userEntity in userList)
                    {
                        trainerManagerList.Add(new ClassManagerAndTrainer
                        {
                            Id = userEntity.Id,
                            Name = userEntity.FullName,
                            Email = userEntity.Email,
                            Role = userEntity.Role
                        });
                    }
                }

                var classInformation = new ClassInformationResModel
                {
                    Id = Class.Id,
                    Name = Class.Name,
                    ProgramDetails = classProgramDetail,
                    StartAt = Class.StartAt,
                    EndAt = Class.EndAt,
                    CreatedAt = Class.CreatedAt,
                    CreatedBy = CreatorClassBy,
                    UpdatedAt = Class.UpdatedAt,
                    UpdatedBy = UpdatorClassBy,
                    NumberOfStudents = _studentClassRepository.GetNumberOfStudents(Class.Id),
                    ListStudent = ListResStudents,
                    Location = Class.Location,
                    Status = Class.Status,
                    classManagerAndTrainers = trainerManagerList
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = classInformation;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateClass(ClassReqModel createForm, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var user = await _userRepository.GetUserById(userId);
                var classExists = await _classRepository.GetClassByName(createForm.Name);
                if (classExists != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Class with the same name already exists!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ClassReqModel, Class>();
                    });
                    IMapper mapper = config.CreateMapper();
                    Class newClass = mapper.Map<ClassReqModel, Class>(createForm);
                    newClass.Name = createForm.Name;
                    newClass.ProgramId = createForm.ProgramId;
                    newClass.StartAt = createForm.StartAt;
                    newClass.EndAt = createForm.EndAt;
                    newClass.CreatedAt = DateTime.Now;
                    newClass.CreatedBy = userId;
                    newClass.Location = createForm.Location;
                    newClass.Status = GeneralStatus.ACTIVE;

                    await _classRepository.Insert(newClass);

                    ClassManager classManager = new ClassManager
                    {
                        ClassId = newClass.Id,
                        UserId = userId,
                        Status = GeneralStatus.ACTIVE
                };

                    await _classManagerRepository.Insert(classManager);

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Class created successfully!";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400; 
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateClass(ClassUpdateReqModel updateForm, Guid classId)
        {
            ResultModel result = new ResultModel();
            try
            {
                // Fetch the existing class entity from the repository
                var existingClass = await _classRepository.GetClassById(classId);
                if (existingClass == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404; // Not Found
                    result.Message = "Class not found!";
                    return result;
                }

                // Map only the properties that can be updated
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ClassUpdateReqModel, Class>()
                       .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => existingClass.CreatedAt))
                       .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => existingClass.CreatedBy));
                });
                IMapper mapper = config.CreateMapper();

                // Map the updated properties onto the existing entity
                mapper.Map(updateForm, existingClass);

                // Update the entity in the repository
                await _classRepository.Update(existingClass);

                //Update status of class manager
                var classManager = await _classManagerRepository.GetClassManagerByClassId(classId);
                if (classManager != null)
                {

                    classManager.Status = existingClass.Status;
                }
                await _classManagerRepository.Update(classManager);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Class updated successfully!";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400; // Internal server error
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DeleteClass(DeleteClassReqModel reqModel)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var classList = await _classRepository.GetListClassById(reqModel.ClassId);
                if (classList == null || classList.Count == 0)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    resultModel.Message = "Class Not Found";
                    return resultModel;
                }

                foreach (var classOb in classList)
                {
                    classOb.Status = UserStatus.INACTIVE;
                }

                await _classRepository.UpdateRange(classList);

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Message = "Delete successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> AddStudentToClass(AddStudentToClassReqModel reqModel, Guid classId)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var studentList = new List<StudentClass>();
                foreach (var studentId in reqModel.StudentId)
                {
                    studentList.Add(new StudentClass
                    {
                        ClassId = classId,
                        StudentId = studentId,
                        Status = UserStatus.ACTIVE
                    });
                }

                await _studentClassRepository.AddRange(studentList);
                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Message = "Add successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> AddTrainerToClass(AddTrainerModel reqModel, Guid classId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var existed = await _classTrainerRepository.GetClassTrainerByClassId(classId);

                if (existed != null)
                {
                    existed.UserId = reqModel.TrainerId;
                    await _classTrainerRepository.Update(existed);
                }
                else
                {
                    var classTrainer = new ClassTrainer
                    {
                        ClassId = classId,
                        UserId = reqModel.TrainerId,
                        Status = GeneralStatus.ACTIVE
                    };
                    await _classTrainerRepository.Insert(classTrainer);
                }

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Message = "Add successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetClassOfStudent(Guid userId)
        {
            ResultModel result = new ResultModel();

            try
            {
                var classes = await _studentClassRepository.GetClassByStudentId(userId);

                if (classes == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Student does not include in any Class.";
                    return result;
                }

                var program = await _trainingProgramRepository.GetTrainingProgramById(classes.ProgramId);
                var createdClassByUser = await _userRepository.GetUserById(classes.CreatedBy);
                var updatedClassByUser = await _userRepository.GetUserById(classes.UpdatedBy);
                var createdProgramByUser = await _userRepository.GetUserById(program.CreatedBy);
                var updatedProgramByUser = await _userRepository.GetUserById(program.UpdatedBy);
                var listResStudents = await _studentClassRepository.GetStudentsOfClass(classes.Id);

                var creatorProgramBy = new CreatorClassOfStudentProgramModel
                {
                    Id = createdProgramByUser.Id,
                    Name = createdProgramByUser.FullName
                };

                var updatorProgramBy = new CreatorClassOfStudentProgramModel();
                if (program.UpdatedBy != null)
                {
                    updatorProgramBy.Id = updatedProgramByUser.Id;
                    updatorProgramBy.Name = updatedProgramByUser.FullName;
                }

                var classOfStudentProgramDetails = new ClassOfStudentProgramDetails
                {
                    Name = program.Name,
                    Code = program.Code,
                    Duration = program.Duration,
                    CreatedAt = program.CreatedAt,
                    CreatedBy = creatorProgramBy,
                    UpdatedAt = program.UpdatedAt,
                    UpdatedBy = updatorProgramBy,
                    Status = program.Status
                };

                var creatorClassBy = new CreatorClassOfStudentModel
                {
                    Id = null,
                    Name = null
                };

                var updatorClassBy = new UpdatorClassOfStudentModel();
                if (classes.UpdatedBy != null)
                {
                    updatorClassBy.Id = updatedClassByUser.Id;
                    updatorClassBy.Name = updatedClassByUser.FullName;
                }

                var userList = new List<User>();
                var classManage = await _classManagerRepository.GetClassManagerByClassId(classes.Id);
                var classTrain = await _classTrainerRepository.GetClassTrainerByClassId(classes.Id);

                var admin = await _userRepository.Get(classManage.UserId);
                var trainer = await _userRepository.Get(classTrain.UserId);

                if (admin != null)
                {
                    userList.Add(admin);
                }

                if (trainer != null)
                {
                    userList.Add(trainer);
                }

                var trainerManagerList = new List<ClassManagerAndTrainer>();

                if (userList.Count > 0)
                {
                    foreach (var user in userList)
                    {
                        trainerManagerList.Add(new ClassManagerAndTrainer
                        {
                            Id = user.Id,
                            Name = user.FullName,
                            Email = user.Email,
                            Role = user.Role
                        });
                    }
                }

                var classOfStudentModel = new ClassOfStudentModel
                {
                    Id = classes.Id,
                    Name = classes.Name,
                    StartAt = classes.StartAt,
                    EndAt = classes.EndAt,
                    CreatedAt = classes.CreatedAt,
                    CreatedBy = creatorClassBy,
                    UpdatedAt = classes.UpdatedAt,
                    UpdatedBy = updatorClassBy,
                    NumberOfStudents = _studentClassRepository.GetNumberOfStudents(classes.Id),
                    ListStudent = listResStudents,
                    Location = classes.Location,
                    Status = classes.Status,
                    ProgramDetails = classOfStudentProgramDetails,
                    classManagerAndTrainers = trainerManagerList.Count > 0 ? trainerManagerList : null
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = classOfStudentModel;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetStudentsByClassId(Guid classId, int page)
        {
            try
            {
                var students = await _userRepository.GetStudentsByClassId(classId);

                if (students == null || !students.Any())
                {
                    return new ResultModel { IsSuccess = false, Message = "No students found for the given class ID", Code = 404 };
                }
                var userProfiles = students.Select(user => new
                {
                    User_ID = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    Dob = user.Dob,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    Role = user.Role,
                }).ToList();

                if (page == null || page == 0)
                {
                    page = 1;
                }

                var ResultList = await Pagination.GetPagination(userProfiles, page, 10);
                return new ResultModel { IsSuccess = true, Data = ResultList, Code = 200 };
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, Message = ex.Message, Code = 400 };
            }
        }

        public async Task<ResultModel> DeleteStudentsFromClass(Guid ClassId, List<Guid> StudentsId)
        {
            ResultModel Result = new();
            try
            {
                var AllStudentsInClass = await _studentClassRepository.GetAllStudentsClassByClassId(ClassId);
                if (AllStudentsInClass == null || !AllStudentsInClass.Any())
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "No students found for the given class ID";
                    return Result;
                }
                var StudentsInClassDelete = AllStudentsInClass.Where(s => StudentsId.Contains(s.StudentId)).ToList();
                await _studentClassRepository.DeleteRange(StudentsInClassDelete);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = $"Delete {StudentsInClassDelete.Count} student(s) successfully";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetListModuleLectureByClassId(Guid classId)
        {
            ResultModel Result = new ResultModel();
            try
            {
                var listRes = await _classRepository.GetModuleLecturesByClassId(classId);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = listRes;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetListModuleLecture(string token)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var Class = await _classRepository.GetClassByUserId(new Guid(Encoder.DecodeToken(token, "userid")));
                if (Class == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    resultModel.Message = "Class not found";
                    return resultModel;
                }
                var Modules = await _moduleRepository.GetModuleByProgramId(Class.ProgramId);
                FilterModel reqModel = new();
                var AllUser = await _userRepository.GetAllUser(reqModel);
                var AllLectures = await _lectureRepository.GetAllLectures();
                var AllDocument = await _documentRepository.GetAll();
                List<ModuleLectureResModel> listRes = new();
                foreach (var module in Modules)
                {
                    ModuleLectureResModel NewModuleLecture = new();
                    NewModuleLecture.Id = module.Id;
                    NewModuleLecture.Name = module.Name;
                    NewModuleLecture.Code = module.Code;
                    NewModuleLecture.Status = module.Status;
                    NewModuleLecture.CreatedAt = module.CreatedAt;
                    NewModuleLecture.CreatedBy = AllUser.Where(x => x.Id.Equals(module.CreatedBy)).Select(u => new AuthorModuleResModel
                    {
                        Id = u.Id,
                        Name = u.FullName
                    }).FirstOrDefault();
                    var ListModuleLecture = AllLectures.Where(l => l.ModuleId.Equals(module.Id)).ToList();
                    var DocumentLecture = AllDocument.Where(d => ListModuleLecture.Select(l => l.Id).Contains(d.LectureId)).ToList();
                    NewModuleLecture.Lectures = ListModuleLecture.Select(l => new LectureOfModuleResModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        CreatedAt = l.CreatedAt,
                        CreatedBy = AllUser.Where(x => x.Id.Equals(l.CreatedBy)).Select(u => new AuthorModuleResModel
                        {
                            Id = u.Id,
                            Name = u.FullName
                        }).FirstOrDefault(),
                        Documents = DocumentLecture.Where(d => d.LectureId.Equals(l.Id)).Select(d => new LectureDocumentModel
                        {
                            Id = d.Id,
                            FileName = d.FileName,
                            CreatedAt = d.CreatedAt,
                            CreatedBy = AllUser.Where(x => x.Id.Equals(d.CreatedBy)).Select(u => new AuthorModuleResModel
                            {
                                Id = u.Id,
                                Name = u.FullName
                            }).FirstOrDefault(),
                        }).ToList()
                    }).ToList();
                    listRes.Add(NewModuleLecture);
                }
                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = listRes;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetClassesOfManagerAndTrainer(string token)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var result = await _studentClassRepository.GetClassInfoByOtherId(userId);
                if (result == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    return resultModel;
                }

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = result;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = e.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetNoneTrainerClassList()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var result = await _classTrainerRepository.GetNoneTrainerClasses();
                if (result.Count == 0)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    resultModel.Message = "Not Found";
                    return resultModel;
                }

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Data = result;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = e.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetModuleListAvgScoreOfClass(Guid classId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var result = await _studentClassRepository.GetModuleListAvgScore(classId);

                if(result == null)
                {
                    resultModel.IsSuccess = true;
                    resultModel.Code = 200;
                    resultModel.Message = "No record";
                    return resultModel;
                }

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = result;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = e.Message;
            }
            return resultModel;
        }
    }
}