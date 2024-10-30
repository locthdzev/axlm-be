using AutoMapper;
using Data.Entities;
using Data.Models.ModuleModel;
using Data.Models.ResultModel;
using Data.Utilities.Pagination;
using Repositories.ClassRepositories;
using Repositories.LectureRepositories;
using Repositories.ModuleProgramRepositories;
using Repositories.ModuleRepositories;
using Repositories.StudentClassRepositories;
using Repositories.SubmissionRepositories;
using Repositories.UserRepositories;
using Encoder = Data.Utilities.Encoder.Encoder;
using static Data.Enums.Status;
using Data.Models.StudentClassModel;
using Data.Models.AssignmentModel;
using System.Net;

namespace ModuleAPIServices.Services
{
    public class ModuleServices : IModuleServices
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IClassRepository _classRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IModuleProgramRepository _moduleProgramRepository;
        private readonly ILectureRepository _lectureRepository;

        public ModuleServices(IModuleRepository moduleRepository, IUserRepository userRepository, ISubmissionRepository submissionRepository, IClassRepository classRepository, IStudentClassRepository studentClassRepository, IModuleProgramRepository moduleProgramRepository, ILectureRepository lectureRepository)
        {
            _moduleRepository = moduleRepository;
            _userRepository = userRepository;
            _submissionRepository = submissionRepository;
            _classRepository = classRepository;
            _studentClassRepository = studentClassRepository;
            _moduleProgramRepository = moduleProgramRepository;
            _lectureRepository = lectureRepository;
        }

        public async Task<ResultModel> GetListModule(int page)
        {
            ResultModel result = new();
            try
            {
                if (page <= 0)
                {
                    page = 1;
                }

                var module = await _moduleRepository.GetModule();
                List<ModuleGeneralResModel> moduleList = new();
                foreach (var m in module)
                {
                    var Creator = await _userRepository.GetUserById(m.CreatedBy);
                    AuthorModuleResModel CreatedBy = new() { Id = Creator.Id, Name = Creator.FullName };
                    ModuleGeneralResModel mdl = new()
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Code = m.Code,
                        CreatedAt = m.CreatedAt,
                        CreatedBy = CreatedBy,
                        Status = m.Status
                    };
                    moduleList.Add(mdl);

                }
                var ResultList = await Pagination.GetPagination(moduleList, page, 10);

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

        public async Task<ResultModel> GetDetailModule(Guid id)
        {
            ResultModel Result = new();
            try
            {
                var Module = await _moduleRepository.GetModuleById(id);
                if (Module == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "No data found";
                    return Result;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Module, ModuleDetailsResModel>()
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                });

                IMapper mapper = config.CreateMapper();
                ModuleDetailsResModel ResModule = mapper.Map<Module, ModuleDetailsResModel>(Module);

                var AmountOfLecture = await _lectureRepository.GetLectureByModuleId(Module.Id);
                var Author = await _userRepository.GetUserById(Module.CreatedBy);
                AuthorModuleResModel AuthorResModel = new()
                {
                    Id = Author.Id,
                    Name = Author.FullName
                };

                AuthorModuleResModel UpdateAuthorResModel = new();
                ResModule.CreatedBy = AuthorResModel;
                ResModule.UpdatedBy = null;
                ResModule.TotalLecture = AmountOfLecture.Count;
                if (Module.UpdatedBy != null)
                {
                    var UpdateAuthor = await _userRepository.GetUserById(Module.UpdatedBy);
                    UpdateAuthorResModel.Id = UpdateAuthor.Id;
                    UpdateAuthorResModel.Name = UpdateAuthor.FullName;
                    ResModule.UpdatedBy = UpdateAuthorResModel;
                }
                if (Module != null)
                {
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Data = ResModule;
                }
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> CreateModule(ModuleReqModel createForm)
        {
            ResultModel result = new ResultModel();
            try
            {
                var ModuleExist = await _moduleRepository.GetModuleByName(createForm.Name);
                if (ModuleExist != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Module with the same name already exists!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ModuleReqModel, Module>();
                    });
                    IMapper mapper = config.CreateMapper();
                    Module newModule = mapper.Map<ModuleReqModel, Module>(createForm);
                    newModule.Status = GeneralStatus.ACTIVE;
                    await _moduleRepository.Insert(newModule);

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Module created successfully!";
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

        public async Task<ResultModel> UpdateModule(ModuleUpdateModel? moduleUpdateModel, Guid moduleId, string token)
        {
            ResultModel Result = new();
            Guid UserId = new Guid(Encoder.DecodeToken(token, "userid"));
            try
            {
                if (moduleUpdateModel is null)
                {
                    throw new ArgumentException(nameof(moduleUpdateModel));
                }

                var moduleUpdate = await _moduleRepository.GetModuleById(moduleId);
                if (moduleUpdate is null)
                {
                    throw new ArgumentException("Given module Id doesn't exist!");
                }

                moduleUpdate.Name = moduleUpdateModel.Name;
                moduleUpdate.Code = moduleUpdateModel.Code;
                moduleUpdate.UpdatedAt = DateTime.Now;
                moduleUpdate.UpdatedBy = UserId;

                await _moduleRepository.Update(moduleUpdate);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Update module successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return Result;
        }

        public async Task<ResultModel> DeleteModule(List<Guid> ListId)
        {
            ResultModel Result = new();
            try
            {
                var Module = await _moduleRepository.GetListModuleById(ListId);
                if (Module.Count == 0)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "There is no module to delete";
                    return Result;
                }

                var moduleProgramList = new List<ModuleProgram>();

                foreach (var m in Module)
                {
                    m.Status = GeneralStatus.INACTIVE;
                    moduleProgramList.AddRange(await _moduleProgramRepository.GetModuleProgramByModuleId(m.Id));
                }

                if(moduleProgramList.Count > 0)
                {
                    await _moduleProgramRepository.DeleteRange(moduleProgramList);
                }

                await _moduleRepository.UpdateRange(Module);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Delete module successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> ViewStudentResultRecord(Guid moduleId, Guid classId)
        {
            ResultModel resultModel = new ResultModel();

            StudentResultRecordListByClassAndModule studentRecordResult = new StudentResultRecordListByClassAndModule();

            Dictionary<StudentResultRecordResModel, double> studentAverageScoreDictionary = new Dictionary<StudentResultRecordResModel, double>();
            List<int> existedRank = new List<int>();
            try
            {
                Class studentClass = await _classRepository.GetClassById(classId);

                List<StudentResultRecordResModel> studentResultRecordResModelList = new List<StudentResultRecordResModel>();

                List<User> studentList = await _studentClassRepository.GetStudentClassByClassId(classId);
                List<Submission> submitOfModule = await _submissionRepository.GetSubmissionListOfModuleId(moduleId);

                foreach (var student in studentList)
                {
                    List<Submission> submissionList = await _submissionRepository.GetSubmissionListByStudentId(student.Id);
                    submissionList = submissionList.Intersect(submitOfModule).ToList();
                    if (submissionList != null && submissionList.Count > 0)
                    {
                        decimal totalScore = 0;
                        int submissionCount = 0;
                        double averageScore = 0.0;

                        StudentResultRecordResModel studentModel = new StudentResultRecordResModel
                        {
                            StudentName = student.FullName,
                            AssignmentModelList = new List<AssignmentTitleAndScoreResModel>(),
                            Average = 0,
                            Rank = 0
                        };

                        studentResultRecordResModelList.Add(studentModel);

                        foreach (var submission in submissionList)
                        {
                            submissionCount++;
                            totalScore += submission.Score;

                            AssignmentTitleAndScoreResModel assignmentTitleAndScoreResModel = new AssignmentTitleAndScoreResModel()
                            {
                                AssignmentId = submission.Assignment.Id,
                                Title = submission.Assignment.Title,
                                Score = submission.Score
                            };

                            studentModel.AssignmentModelList.Add(assignmentTitleAndScoreResModel);
                        }

                        averageScore = (double)totalScore / submissionCount;

                        studentAverageScoreDictionary.Add(studentModel, averageScore);
                        studentModel.Average = averageScore;
                    }
                }

                studentRecordResult.ClassName = studentClass.Name;
                studentRecordResult.StudentModelList = studentResultRecordResModelList;

                var sortedStudentScoreDictionary = studentAverageScoreDictionary
                                        .OrderByDescending(x => x.Value)
                                        .ToDictionary(x => x.Key, x => x.Value);

                int rank = 1;
                existedRank.Add(rank);

                int existedRankGap = 1;

                foreach (var item in sortedStudentScoreDictionary)
                {
                    item.Key.Rank = rank;
                    if (!existedRank.Contains(rank))
                    {
                        existedRank.Add(rank);
                        rank += existedRankGap;
                    }
                    else
                    {
                        existedRankGap++;
                    }
                }

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Data = studentRecordResult;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> AddModuleByCopy(Guid programId, List<Guid> moduleIdList, string token)
        {
            var resultModel = new ResultModel();
            var userId = new Guid(Encoder.DecodeToken(token, "userid"));
            try
            {
                var newModuleProgramList = new List<ModuleProgram>();

                foreach (var moduleId in moduleIdList)
                {
                    var newModule = await _moduleRepository.GetModuleById(moduleId);
                    if (newModule != null)
                    {
                        newModuleProgramList.Add(new ModuleProgram
                        {
                            ProgramId = programId,
                            ModuleId = moduleId,
                            Status = GeneralStatus.ACTIVE
                        });
                    }
                    else
                    {
                        return new ResultModel
                        {
                            Code = 400,
                            IsSuccess = false,
                            Message = $"Cannot find module with id {moduleId}"
                        };
                    }
                }

                await _moduleProgramRepository.AddRange(newModuleProgramList);

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
    }
}