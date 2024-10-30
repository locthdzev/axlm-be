using System.Net;
using AutoMapper;
using Data.Entities;
using Data.Models.LectureModel;
using Data.Models.ResultModel;
using Data.Utilities.CloudStorage;
using Data.Utilities.Pagination;
using Repositories.DocumentRepositories;
using Repositories.LectureRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;

namespace LectureAPIServices.Services
{
    public class LectureServices : ILectureServices
    {
        private readonly ILectureRepository _lectureRepository;
        private readonly IDocumentRepository _documentRepo;
        private readonly IUserRepository _userRepository;
        private readonly ICloudStorage _firebaseStorage;

        public LectureServices(ILectureRepository lectureRepository, IDocumentRepository documentRepo, IUserRepository userRepository, ICloudStorage firebaseStorage)
        {
            _lectureRepository = lectureRepository;
            _documentRepo = documentRepo;
            _userRepository = userRepository;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<ResultModel> GetListLecture(int page)
        {
            ResultModel result = new();
            try
            {
                if (page <= 0)
                {
                    page = 1;
                }

                var lecture = await _lectureRepository.GetListLectures();
                List<LectureResModel> lectureList = new();
                foreach (var l in lecture)
                {
                    var Creator = await _userRepository.GetUserById(l.CreatedBy);
                    AuthorLectureResModel CreatedBy = new() { Id = Creator.Id, Name = Creator.FullName, };
                    LectureResModel lt = new()
                    {
                        Id = l.Id,
                        Order = l.Order,
                        ModuleId = l.ModuleId,
                        ClassId = l.ClassId,
                        Name = l.Name,
                        CreatedAt = l.CreatedAt,
                        CreatedBy = CreatedBy,
                        Status = l.Status
                    };
                    lectureList.Add(lt);

                }
                var ResultList = await Pagination.GetPagination(lectureList, page, 10);

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

        public async Task<ResultModel> CreateLecture(LectureReqModel createForm)
        {
            ResultModel result = new();
            try
            {
                var LectureExist = await _lectureRepository.GetLectureByName(createForm.Name);
                if (LectureExist != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Lecture with the same name already exists!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<LectureReqModel, Lecture>();
                    });
                    IMapper mapper = config.CreateMapper();
                    Lecture newLecture = mapper.Map<LectureReqModel, Lecture>(createForm);
                    Guid LectureId = Guid.NewGuid();
                    newLecture.Id = LectureId;
                    newLecture.Status = GeneralStatus.ACTIVE;
                    await _lectureRepository.Insert(newLecture);
                    if (createForm.Files != null && createForm.Files.Count > 0)
                    {
                        string filePath = $"class_{createForm.ClassId}/documents/module_{createForm.ModuleId}/lecture_{LectureId}";
                        List<Document> documents = new();
                        var fileName = await _firebaseStorage.UploadFilesToFirebase(createForm.Files, filePath);
                        foreach (var name in fileName)
                        {
                            documents.Add(new Document
                            {
                                LectureId = LectureId,
                                FileName = name,
                                CreatedAt = DateTime.Now,
                                CreatedBy = createForm.CreatedBy,
                                Status = GeneralStatus.ACTIVE
                            });
                        }
                        await _documentRepo.AddRange(documents);
                    }

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Lecture created successfully!";
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

        public async Task<ResultModel> UpdateLecture(LectureUpdateModel lectureUpdateRequest, Guid lectureId, string token)
        {
            ResultModel Result = new();
            try
            {
                if (lectureUpdateRequest is null)
                {
                    throw new ArgumentNullException(nameof(lectureUpdateRequest));
                }

                Guid UserId = new Guid(Encoder.DecodeToken(token, "userid"));
                var lectureUpdate = await _lectureRepository.GetLectureById(lectureId);
                if (lectureUpdate is null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    return Result;
                }
                var ListDocuments = await _documentRepo.GetListDocumentByLectureId(lectureId);
                string filePath = $"class_{lectureUpdate.ClassId}/documents/module_{lectureUpdate.ModuleId}/lecture_{lectureId}";
                foreach (var Document in ListDocuments)
                {
                    await _firebaseStorage.RemoveFileFromFirebase(Document.FileName, filePath);
                }
                await _documentRepo.DeleteRange(ListDocuments);
                if (lectureUpdateRequest.Files != null && lectureUpdateRequest.Files.Count > 0)
                {
                    List<Document> Documents = new();
                    var fileName = await _firebaseStorage.UploadFilesToFirebase(lectureUpdateRequest.Files, filePath);
                    foreach (var name in fileName)
                    {
                        Documents.Add(new Document
                        {
                            LectureId = lectureId,
                            CreatedAt = DateTime.Now,
                            CreatedBy = UserId,
                            FileName = name,
                            Status = GeneralStatus.ACTIVE
                        });
                    }
                    await _documentRepo.AddRange(Documents);
                }

                lectureUpdate.Order = 0;
                lectureUpdate.Name = lectureUpdateRequest.Name;
                lectureUpdate.UpdatedAt = DateTime.Now;
                lectureUpdate.UpdatedBy = UserId;

                await _lectureRepository.Update(lectureUpdate);

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Update lecture successful!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return Result;
        }

        public async Task<ResultModel> GetLectureDetail(Guid id)
        {
            ResultModel Result = new();
            try
            {
                var Lecture = await _lectureRepository.GetLectureById(id);
                if (Lecture == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.ResponseFailed = "No data found";
                    return Result;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Lecture, LectureDetailResModel>()
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                });

                IMapper mapper = config.CreateMapper();
                LectureDetailResModel ResLecture = mapper.Map<Lecture, LectureDetailResModel>(Lecture);

                var Author = await _userRepository.GetUserById(Lecture.CreatedBy);
                AuthorLectureResModel AuthorResModel = new()
                {
                    Id = Author.Id,
                    Name = Author.FullName
                };

                AuthorLectureResModel UpdateAuthorResModel = new();
                ResLecture.CreatedBy = AuthorResModel;
                ResLecture.UpdatedBy = null;
                if (Lecture.UpdatedBy != null)
                {
                    var UpdateAuthor = await _userRepository.GetUserById(Lecture.UpdatedBy);
                    UpdateAuthorResModel.Id = UpdateAuthor.Id;
                    UpdateAuthorResModel.Name = UpdateAuthor.FullName;
                    ResLecture.UpdatedBy = UpdateAuthorResModel;
                }
                if (Lecture != null)
                {
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Data = ResLecture;
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

        public async Task<ResultModel> DeleteLecture(List<Guid> ListId)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var lectureList = await _lectureRepository.GetListLecturesById(ListId);
                if (lectureList.Count == 0)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 404;
                    return resultModel;
                }

                var listDocuments = new List<Document>();
                foreach (var lecture in lectureList)
                {
                    lecture.Status = GeneralStatus.INACTIVE;
                    listDocuments.AddRange(await _documentRepo.GetListDocumentByLectureId(lecture.Id));
                    string filePath = $"class_{lecture.ClassId}/documents/module_{lecture.ModuleId}/lecture_{lecture.Id}";
                    foreach (var document in listDocuments)
                    {
                        await _firebaseStorage.RemoveFileFromFirebase(document.FileName, filePath);
                    }
                    await _documentRepo.DeleteRange(listDocuments);
                }

                await _lectureRepository.DeleteRange(lectureList);

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
    }
}