using System.Net;
using AutoMapper;
using Data.Entities;
using Data.Models.ModuleModel;
using Data.Models.ResultModel;
using Data.Models.TrainingProgramModel;
using Data.Utilities.Pagination;
using Repositories.ClassRepositories;
using Repositories.ModuleProgramRepositories;
using Repositories.ModuleRepositories;
using Repositories.TrainingProgramRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace TrainingProgramAPIServices.Services
{
    public class TrainingProgramServices : ITrainingProgramServices
    {
        private readonly ITrainingProgramRepository _trainingProgramRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClassRepository _classRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IModuleProgramRepository _moduleProgramRepository;

        public TrainingProgramServices(ITrainingProgramRepository trainingProgramRepository, IUserRepository userRepository, IClassRepository classRepository, IModuleRepository moduleRepository, IModuleProgramRepository moduleProgramRepository)
        {
            _trainingProgramRepository = trainingProgramRepository;
            _userRepository = userRepository;
            _classRepository = classRepository;
            _moduleRepository = moduleRepository;
            _moduleProgramRepository = moduleProgramRepository;
        }

        public async Task<ResultModel> CreateTrainingProgram(TrainingProgramReqModel? createForm)
        {
            ResultModel result = new();
            try
            {
                var TrainingProgramExist = await _trainingProgramRepository.GetTrainingProgramByName(createForm.Name);
                if (TrainingProgramExist != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Training Program with the same name already exists!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<TrainingProgramReqModel, TrainingProgram>();
                    });
                    IMapper mapper = config.CreateMapper();
                    TrainingProgram newTrainingProgram = mapper.Map<TrainingProgramReqModel, TrainingProgram>(createForm);
                    newTrainingProgram.Status = GeneralStatus.ACTIVE;
                    await _trainingProgramRepository.Insert(newTrainingProgram);

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Training Program created successfully!";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateTrainingProgram(TrainingProgramResModel UpdateTrainingProgram, Guid programId)
        {
            ResultModel Result = new();
            try
            {
                ArgumentNullException.ThrowIfNull(UpdateTrainingProgram);

                var trainingprogramUpdate = await _trainingProgramRepository.GetTrainingProgramById(programId) ?? throw new ArgumentException("Given training program Id doesn't exist!");
                trainingprogramUpdate.Name = UpdateTrainingProgram.Name;
                trainingprogramUpdate.Code = UpdateTrainingProgram.Code;
                trainingprogramUpdate.Duration = UpdateTrainingProgram.Duration;


                await _trainingProgramRepository.Update(trainingprogramUpdate);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Update training program successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }


            return Result;
        }
        public async Task<ResultModel> GetListProgramTraining()
        {
            ResultModel Result = new();
            try
            {
                var listProgramTraining = await _trainingProgramRepository.GetListProgramTraining();
                if (listProgramTraining.Count > 0)
                {
                    List<TrainingProgramListResModel> TrainingPrograms = new();
                    foreach (var item in listProgramTraining)
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<TrainingProgram, TrainingProgramListResModel>().ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                        });
                        IMapper mapper = config.CreateMapper();
                        TrainingProgramListResModel TrainingProgramListResModel = mapper.Map<TrainingProgram, TrainingProgramListResModel>(item);
                        TrainingPrograms.Add(TrainingProgramListResModel);
                        var Author = await _userRepository.GetUserById(item.CreatedBy);
                        var AmountOfClass = await _classRepository.GetListClassByTrainingProgramId(item.Id);
                        var AuthorResModel = new AuthorResModel();
                        AuthorResModel.Id = Author.Id;
                        AuthorResModel.FullName = Author.FullName;
                        TrainingProgramListResModel.TotalClass = AmountOfClass.Count;
                        TrainingProgramListResModel.CreatedBy = AuthorResModel;
                    }
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Data = TrainingPrograms;
                }
                else
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "No data found";
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


        public async Task<ResultModel> DeleteTrainingProgram(List<Guid> ListId)
        {
            ResultModel Result = new();
            try
            {
                var ProgramTraining = await _trainingProgramRepository.GetListProgramTrainingByListId(ListId);
                if (ProgramTraining.Count == 0)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "There is no training program to delete";
                    return Result;
                }

                foreach (var pt in ProgramTraining)
                {
                    pt.Status = GeneralStatus.INACTIVE;
                    var moduleProgramList = await _moduleProgramRepository.GetModuleProgramByProgramId(pt.Id);
                    if (moduleProgramList.Count > 0)
                    {
                        await _moduleProgramRepository.DeleteRange(moduleProgramList);
                    }
                }

                await _trainingProgramRepository.UpdateRange(ProgramTraining);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Delete training program successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetListDropDownProgramTraining()
        {
            ResultModel Result = new();
            try
            {
                var listProgramTraining = await _trainingProgramRepository.GetListDropDownProgramTraining();
                if (listProgramTraining.Count > 0)
                {
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Data = listProgramTraining;
                }
                else
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "No data found";
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

        public async Task<ResultModel> GetModuleByProgramId(Guid programId, int page)
        {
            ResultModel result = new();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var module = await _moduleProgramRepository.GetModulesByProgramId(programId);
                List<ModuleGeneralResModel> moduleList = new();
                if (module == null || !module.Any())
                {
                    return new ResultModel { IsSuccess = false, Code = 404, Message = "No modules found" };
                }
                foreach (var m in module)
                {
                    var Creator = await _userRepository.GetUserById(m.Module.CreatedBy);
                    AuthorModuleResModel CreatedBy = new() { Id = Creator.Id, Name = Creator.FullName, };
                    ModuleGeneralResModel mdl = new()
                    {
                        Id = m.Module.Id,
                        Name = m.Module.Name,
                        Code = m.Module.Code,
                        CreatedAt = m.Module.CreatedAt,
                        CreatedBy = CreatedBy,
                        Status = m.Module.Status
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

        public async Task<ResultModel> GetProgramTraining(Guid Id)
        {
            ResultModel Result = new();
            try
            {
                var ProgramTraining = await _trainingProgramRepository.GetTrainingProgramById(Id);
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TrainingProgram, TrainingProgramDetailsResModel>().ForMember(dest => dest.CreatedBy, opt => opt.Ignore()).ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                });
                IMapper mapper = config.CreateMapper();
                TrainingProgramDetailsResModel ResProgramTraining = mapper.Map<TrainingProgram, TrainingProgramDetailsResModel>(ProgramTraining);
                var AmountOfClass = await _classRepository.GetListClassByTrainingProgramId(ProgramTraining.Id);
                var Author = await _userRepository.GetUserById(ProgramTraining.CreatedBy);
                AuthorResModel AuthorResModel = new();
                AuthorResModel.Id = Author.Id;
                AuthorResModel.FullName = Author.FullName;
                AuthorResModel UpdateAuthorResModel = new();
                ResProgramTraining.CreatedBy = AuthorResModel;
                ResProgramTraining.UpdatedBy = null;
                ResProgramTraining.TotalClass = AmountOfClass.Count;
                ResProgramTraining.ClassList = await _classRepository.GetClassesByTrainingProgramId(Id);
                if (ProgramTraining.UpdatedBy != null)
                {
                    var UpdateAuthor = await _userRepository.GetUserById(ProgramTraining.UpdatedBy);
                    UpdateAuthorResModel.Id = UpdateAuthor.Id;
                    UpdateAuthorResModel.FullName = UpdateAuthor.FullName;
                    ResProgramTraining.UpdatedBy = UpdateAuthorResModel;
                }
                if (ProgramTraining != null)
                {
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Data = ResProgramTraining;
                }
                else
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "No data found";
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

        public async Task<ResultModel> GetModuleOfOthers(Guid programId, int page)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var result = await _moduleProgramRepository.GetModuleOfOtherProgram(programId);

                if (result.Count == 0)
                {
                    return new ResultModel { Code = 404, IsSuccess = false, Message = "Not Found" };
                }

                var finalResult = await Pagination.GetPagination(result, page, 10);

                resultModel.IsSuccess = true;
                resultModel.Data = finalResult;
                resultModel.Code = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> CreateAndAddModuleToTP(string token, Guid programId, List<ModuleReqCreateAndAddToTP> ListModuleInfo)
        {
            ResultModel Result = new();
            try
            {
                var userId = Guid.Parse(Encoder.DecodeToken(token, "userid"));
                var AllModules = await _moduleRepository.GetAllModules();
                var AllModuleProgram = await _moduleProgramRepository.GetModuleProgramByProgramId(programId);
                List<Module> AddModule = new();
                List<ModuleProgram> AddModuleProgram = new();
                List<string> ListIgnoreCreate = new();
                foreach (var item in ListModuleInfo)
                {
                    if (AllModules.Any(x => x.Name.Equals(item.Name)))
                    {
                        var ModuleId = AllModules.Where(x => x.Name.Equals(item.Name)).FirstOrDefault().Id;
                        if (!AllModuleProgram.Any(x => x.ModuleId.Equals(ModuleId)))
                        {
                            AddModuleProgram.Add(new ModuleProgram()
                            {
                                Id = Guid.NewGuid(),
                                ModuleId = ModuleId,
                                ProgramId = programId,
                                Status = GeneralStatus.ACTIVE
                            });
                        }
                        ListIgnoreCreate.Add(item.Name);
                    }
                    else
                    {
                        var NewModuleId = Guid.NewGuid();
                        AddModule.Add(new Module()
                        {
                            Id = NewModuleId,
                            Name = item.Name,
                            Code = item.Code,
                            CreatedAt = DateTime.Now,
                            CreatedBy = userId,
                            Status = GeneralStatus.ACTIVE
                        });
                        AddModuleProgram.Add(new ModuleProgram()
                        {
                            Id = Guid.NewGuid(),
                            ModuleId = NewModuleId,
                            ProgramId = programId,
                            Status = GeneralStatus.ACTIVE
                        });
                    }
                }
                await _moduleRepository.AddRange(AddModule);
                await _moduleProgramRepository.AddRange(AddModuleProgram);
                if (ListIgnoreCreate.Count != 0)
                {
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Message = "Warning: Some modules name already exist in the system, so they are not created: " + string.Join(", ", ListIgnoreCreate);
                }
                else
                {
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Message = "Create and add module to training program successfully!";
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

        public async Task<ResultModel> DeleteModuleFromTP(string token, Guid programId, List<Guid> ListModuleId)
        {
            ResultModel Result = new();
            try
            {
                var AllModuleProgram = await _moduleProgramRepository.GetModuleProgramByProgramId(programId);
                var ModuleProgramDelete = AllModuleProgram.Where(x => ListModuleId.Contains(x.ModuleId)).ToList();
                if (ModuleProgramDelete.Count == 0)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "There is no module to delete";
                    return Result;
                }
                await _moduleProgramRepository.DeleteRange(ModuleProgramDelete);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = $"Delete {ModuleProgramDelete.Count} module(s) from training program successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }
    }
}