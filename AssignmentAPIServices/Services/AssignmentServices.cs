using System.Net;
using System.Security.Claims;
using AutoMapper;
using Data.Entities;
using Data.Enums;
using Data.Models.AssignmentModel;
using Data.Models.FilterModel;
using Data.Models.ResultModel;
using Data.Models.SubmissionModel;
using Data.Utilities.CloudStorage;
using Repositories.AssignmDetailsRepositories;
using Repositories.AssignmentRepositories;
using Repositories.SubmissionRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace AssignmentAPIServices.Services
{
    public class AssignmentServices : IAssignmentServices
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICloudStorage _firebaseStorage;
        private readonly IAssignmDetailsRepository _assignmDetailsRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUserRepository _userRepository;

        public AssignmentServices(IAssignmentRepository assignmentRepository, ICloudStorage firebaseStorage, IAssignmDetailsRepository assignmDetailsRepository, ISubmissionRepository submissionRepository, IUserRepository userRepository)
        {
            _assignmentRepository = assignmentRepository;
            _firebaseStorage = firebaseStorage;
            _assignmDetailsRepository = assignmDetailsRepository;
            _submissionRepository = submissionRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultModel> GetAsmList(Guid ClassId, string token)
        {
            ResultModel Result = new ResultModel();
            try
            {
                string role = Encoder.DecodeToken(token, ClaimsIdentity.DefaultRoleClaimType);
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var result = new List<ModuleAssignmentResModel>();
                if (role.Equals(Roles.STUDENT))
                {
                    result = await _assignmentRepository.GetModuleListWithAssignmentsOfClass(ClassId, userId);
                }
                else
                {
                    result = await _assignmentRepository.GetModuleListWithAssignmentsOfClass(ClassId, Guid.Empty);
                }

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = result;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> CreateAssignment(AssignmentCreateReqModel createForm, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));
                var assignmentExists = await _assignmentRepository.GetAssignmentByTitle(createForm.Title);
                if (assignmentExists != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Assignment with the same title already exists!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<AssignmentCreateReqModel, Assignment>();
                    });
                    IMapper mapper = config.CreateMapper();
                    Assignment newAssignment = mapper.Map<AssignmentCreateReqModel, Assignment>(createForm);
                    var AssignmentId = Guid.NewGuid();
                    newAssignment.CreatedBy = userId;
                    newAssignment.CreatedAt = DateTime.Now;
                    newAssignment.Id = AssignmentId;
                    newAssignment.IsOverTime = false;
                    newAssignment.Status = GeneralStatus.ACTIVE;
                    await _assignmentRepository.Insert(newAssignment);
                    string filePath = $"class_{createForm.ClassId}/assignment_{AssignmentId}";
                    if (createForm.Files != null && createForm.Files.Count > 0)
                    {
                        List<AssignmentDetail> assignmentDetails = new();
                        var fileName = await _firebaseStorage.UploadFilesToFirebase(createForm.Files, filePath);
                        foreach (var name in fileName)
                        {
                            assignmentDetails.Add(new AssignmentDetail
                            {
                                AttachmentUrl = name,
                                AssignmentId = AssignmentId,
                                CreatedAt = DateTime.Now,
                                Status = GeneralStatus.ACTIVE
                            });
                        }
                        await _assignmDetailsRepository.AddRange(assignmentDetails);
                    }

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Assignment created successfully!";
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
        public async Task<ResultModel> UpdateAssignment(Guid id, AssignmentUpdateReqModel updateForm, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                var userId = new Guid(Encoder.DecodeToken(token, "userid"));

                var existingAssignment = await _assignmentRepository.GetAssignmentById(id);
                var AttachmentAssignment = await _assignmDetailsRepository.GetAllAttachmentByAssignmentId(id);
                if (existingAssignment == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Assignment not found";
                    return result;
                }

                existingAssignment.Title = updateForm.Title;
                existingAssignment.Content = updateForm.Content;
                existingAssignment.ExpiryDate = updateForm.ExpiryDate;
                existingAssignment.IsOverTime = updateForm.IsOverTime;
                string filePath = $"class_{existingAssignment.ClassId}/assignment_{id}";
                foreach (var Attachment in AttachmentAssignment)
                {
                    await _firebaseStorage.RemoveFileFromFirebase(Attachment.AttachmentUrl, filePath);
                }
                await _assignmDetailsRepository.DeleteRange(AttachmentAssignment);
                if (updateForm.Files != null && updateForm.Files.Count > 0)
                {
                    List<AssignmentDetail> assignmentDetails = new();
                    var fileName = await _firebaseStorage.UploadFilesToFirebase(updateForm.Files, filePath);
                    foreach (var name in fileName)
                    {
                        assignmentDetails.Add(new AssignmentDetail
                        {
                            AttachmentUrl = name,
                            AssignmentId = id,
                            CreatedAt = DateTime.Now,
                            Status = GeneralStatus.ACTIVE
                        });
                    }
                    await _assignmDetailsRepository.AddRange(assignmentDetails);
                }

                // Save the updated assignment
                await _assignmentRepository.Update(existingAssignment);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Assignment updated successfully!";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> DeleteAssignment(List<Guid> assignmentId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var assignmentList = await _assignmentRepository.GetListAssignmentById(assignmentId);

                if (assignmentList.Count() == 0)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    return result;
                }

                foreach (var assignment in assignmentList)
                {
                    assignment.Status = GeneralStatus.INACTIVE;
                }

                await _assignmentRepository.UpdateRange(assignmentList);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Delete successfully!";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetAssignmentInformation(Guid assignmentId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var Assignment = await _assignmentRepository.GetAssignmentById(assignmentId);

                if (Assignment == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Assignment not found";
                    return result;
                }

                var CreatedAssignmentByUser = await _userRepository.GetUserById(Assignment.CreatedBy);
                var GetList = await _assignmDetailsRepository.GetAllAttachmentByAssignmentId(assignmentId);

                List<AssignmentDetails> AssignmentDetails = new();
                foreach (var attachment in GetList)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<AssignmentDetail, AssignmentDetails>();
                    });
                    IMapper mapper = config.CreateMapper();
                    AssignmentDetails PartOfAssignmentDetails = mapper.Map<AssignmentDetail, AssignmentDetails>(attachment);
                    PartOfAssignmentDetails.FileName = attachment.AttachmentUrl;
                    AssignmentDetails.Add(PartOfAssignmentDetails);
                }

                CreatorAssignmentModel CreatorAssignmentBy = new()
                {
                    Id = CreatedAssignmentByUser.Id,
                    Name = CreatedAssignmentByUser.FullName
                };

                UpdatorAssignmentModel UpdatorAssignmentBy = new();
                if (Assignment.UpdatedBy != null)
                {
                    var UpdatedAssignmentByUser = await _userRepository.GetUserById(Assignment.UpdatedBy);
                    UpdatorAssignmentBy.Id = UpdatedAssignmentByUser.Id;
                    UpdatorAssignmentBy.Name = UpdatedAssignmentByUser.FullName;
                }

                var assignmentInformation = new AssignmentInformationResModel
                {
                    Id = Assignment.Id,
                    ModuleId = Assignment.ModuleId,
                    ClassId = Assignment.ClassId,
                    Title = Assignment.Title,
                    Content = Assignment.Content,
                    ExpiryDate = Assignment.ExpiryDate,
                    IsOverTime = Assignment.IsOverTime,
                    AssignmentDetails = AssignmentDetails,
                    CreatedAt = Assignment.CreatedAt,
                    CreatedBy = CreatorAssignmentBy,
                    UpdatedAt = Assignment.UpdatedAt,
                    UpdatedBy = UpdatorAssignmentBy,
                    Status = Assignment.Status,
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = assignmentInformation;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ExcelTemplateModel> GetExcelTemplateContent(Guid assignmentId)
        {
            ExcelTemplateModel model = new ExcelTemplateModel();
            try
            {
                var assignment = await _assignmentRepository.GetAssignmentById(assignmentId);
                var classEntity = await _assignmentRepository.GetClassByAssignmentId(assignmentId);
                var submissions = await _submissionRepository.CheckSubmissionOfClass(classEntity.Id, assignmentId);

                model.assignment = assignment;
                model.classEntity = classEntity;
                model.checks = submissions;
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public async Task<ResultModel> ImportAssignmentScore(List<ScoreModel> scoreModel, Guid assignmentId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var assignment = await _assignmentRepository.GetAssignmentById(assignmentId);
                if (assignment == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                }
                var idScoreList = new List<IdScoreSubmitModel>();

                foreach (var score in scoreModel)
                {
                    var user = await _userRepository.GetUserByEmail(score.email);
                    if (score.isSubmit.Equals("Submitted"))
                    {
                        idScoreList.Add(new IdScoreSubmitModel
                        {
                            studentId = user.Id,
                            score = score.score,
                            IsSubmit = score.isSubmit,
                        });
                    }
                }

                var updateList = new List<Submission>();
                var addNewList = new List<Submission>();

                foreach (var idScore in idScoreList)
                {
                    var existed = await _submissionRepository.GetSubmissionByStudentAndAssignment(idScore.studentId, assignmentId);
                    if (existed != null)
                    {
                        existed.Score = idScore.score;
                        existed.IsGrade = true;
                        updateList.Add(existed);
                    }
                    else
                    {
                        var newSubmission = new Submission
                        {
                            StudentId = idScore.studentId,
                            AssignmentId = assignmentId,
                            AttachmentUrl = "-",
                            Score = idScore.score,
                            IsGrade = true,
                            CreatedAt = DateTime.Now,
                            Status = GeneralStatus.ACTIVE,
                        };
                        addNewList.Add(newSubmission);
                    }
                }

                if (updateList.Count > 0)
                {
                    await _submissionRepository.UpdateRange(updateList);
                }

                if(addNewList.Count > 0)
                {
                    await _submissionRepository.AddRange(addNewList);
                }

                resultModel.IsSuccess = true;
                resultModel.Code = (int)HttpStatusCode.OK;
                resultModel.Message = "Import successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> SubmitAssignment(SubmissionCreateResModel submissionCreateResModel, Guid assignmentId, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                Guid UserId = new Guid(Encoder.DecodeToken(token, "userid"));
                // Check if the class already exists
                var classExists = await _submissionRepository.GetSubmissionByStudentAndAssignment(UserId, assignmentId);
                if (classExists != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Submit with the same name already exists!";
                }
                else
                {
                    var Assignment = await _assignmentRepository.GetAssignmentById(assignmentId);
                    if (Assignment == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 404;
                        result.Message = "Assignment not found";
                        return result;
                    }
                    //// Mapping createForm to Class object
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SubmissionCreateResModel, Submission>();
                    //});
                    //IMapper mapper = config.CreateMapper();
                    //Submission newSubmit = mapper.Map<SubmissionCreateResModel, Submission>(submissionCreateResModel);
                    string filePath = $"class_{Assignment.ClassId}/assignment_{Assignment.Id}/{UserId}";
                    if (submissionCreateResModel.Attachment != null)
                    {
                        List<IFormFile> submissions = new();
                        List<Submission> submissionDetails = new();
                        submissions.Add(submissionCreateResModel.Attachment);
                        var fileName = await _firebaseStorage.UploadFilesToFirebase(submissions, filePath);
                        foreach (var name in fileName)
                        {
                            submissionDetails.Add(new Submission
                            {
                                Id = Guid.NewGuid(),
                                AssignmentId = assignmentId,
                                StudentId = UserId,
                                AttachmentUrl = name,
                                CreatedAt = DateTime.Now,
                                IsGrade = false,
                                Score = 0,
                                Status = GeneralStatus.ACTIVE
                            });
                        }
                        await _submissionRepository.AddRange(submissionDetails);
                    }
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Submit assignment successfully!";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400; // Internal server error
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateSubmissionAssignment(SubmissionCreateResModel submissionUpdateResModel, Guid AssignmentId, string token)
        {
            ResultModel result = new ResultModel();
            try
            {
                Guid UserId = new Guid(Encoder.DecodeToken(token, "userid"));

                var existingSubmission = await _submissionRepository.GetSubmissionByStudentAndAssignment(UserId, AssignmentId);

                if (existingSubmission == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Submission not found";
                    return result;
                }
                var assignment = await _assignmentRepository.GetAssignmentById(AssignmentId);
                string filePath = $"class_{assignment.ClassId}/assignment_{AssignmentId}/{UserId}";
                await _firebaseStorage.RemoveFileFromFirebase(existingSubmission.AttachmentUrl, filePath);
                List<IFormFile> submissions = new();
                submissions.Add(submissionUpdateResModel.Attachment);
                var fileName = await _firebaseStorage.UploadFilesToFirebase(submissions, filePath);
                existingSubmission.AttachmentUrl = fileName[0];
                existingSubmission.CreatedAt = DateTime.Now;
                await _submissionRepository.Update(existingSubmission);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Submission updated successfully!";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400; // Internal server error
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> ScoreAssignment(ScoreAssignmentResModel scoreAssignmentResModel, Guid assignmentId)
        {
            ResultModel Result = new();
            try
            {
                if (scoreAssignmentResModel is null)
                {
                    throw new ArgumentNullException(nameof(scoreAssignmentResModel));
                }

                var assignmentUpdate = await _submissionRepository.GetSubmissionByStudentAndAssignment(scoreAssignmentResModel.StudentId, assignmentId);

                if (assignmentUpdate is null)
                {
                    throw new ArgumentNullException("Given assignment Id doesn't exist!");
                }
                assignmentUpdate.IsGrade = true;
                assignmentUpdate.Score = scoreAssignmentResModel.Score;
                await _submissionRepository.Update(assignmentUpdate);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Score assignment successfully!";
            }
            catch (Exception ex)
            {
                Result.IsSuccess = false;
                Result.Code = (int)HttpStatusCode.BadRequest;
                Result.Message = ex.Message;
            }
            return Result;
        }

        public async Task<ResultModel> UpdateScoreForAssignment(AssignmentUpdateScoreReqModel AssignmentUpdateScoreReqModel, Guid assignmentId)
        {
            ResultModel Result = new();
            try
            {
                var SubmissionData = await _submissionRepository.GetListSubmissionByAssignmentId(assignmentId);
                if (SubmissionData.Count == 0)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "No submission found";
                    return Result;
                }

                foreach (var submission in SubmissionData)
                {
                    var matchedAssignment = AssignmentUpdateScoreReqModel.ScoresData.FirstOrDefault(s => s.StudentId == submission.StudentId);
                    if (matchedAssignment != null)
                    {
                        submission.Score = matchedAssignment.Score;
                    }
                }
                await _submissionRepository.UpdateRange(SubmissionData);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Update score for assignment successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetSubmissionAssignment(Guid assignmentId, string token)
        {
            ResultModel Result = new ResultModel();
            try
            {
                Guid UserId = new Guid(Encoder.DecodeToken(token, "userid"));
                var SubmissionData = await _submissionRepository.GetSubmissionByStudentAndAssignment(UserId, assignmentId);
                FilterModel reqModel = new();
                var AllUser = await _userRepository.GetAllUser(reqModel);
                if (SubmissionData == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "No submission found";
                    return Result;
                }
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Submission, SubmissionResModel>().ForMember(dest => dest.Student, opt => opt.Ignore()); ;
                });
                IMapper mapper = config.CreateMapper();
                SubmissionResModel SubmissionRes = mapper.Map<Submission, SubmissionResModel>(SubmissionData);
                SubmissionRes.FileName = SubmissionData.AttachmentUrl;
                SubmissionRes.Student = AllUser.Where(x => x.Id.Equals(UserId)).Select(u => new AuthorOfSubmission
                {
                    Id = u.Id,
                    Name = u.FullName
                }).FirstOrDefault();
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = SubmissionRes;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetSubmissionByAssignmentId(Guid id)
        {
            ResultModel Result = new ResultModel();
            try
            {
                var result = await _submissionRepository.GetListStudentWithSubmissions(id);
                if (result.Count == 0)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    return Result;
                }

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = result;
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