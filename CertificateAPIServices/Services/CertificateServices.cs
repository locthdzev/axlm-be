using System.Text;
using AutoMapper;
using Data.Entities;
using Data.Models.CertificateModel;
using Data.Models.ResultModel;
using Data.Models.StudentClassModel;
using Data.Models.SubmissionModel;
using Data.Utilities.CloudStorage;
using Repositories.AttendanceRepositories;
using Repositories.AttendDetaillsRepositories;
using Repositories.CertificateRepositories;
using Repositories.StudentRepositories;
using Repositories.SubmissionRepositories;
using Repositories.TrainingProgramRepositories;
using static Data.Enums.Status;

namespace CertificateAPIServices.Services
{
    public class CertificateServices : ICertificateServices
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITrainingProgramRepository _trainingProgramRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendDetaillsRepository _attendDetaillsRepository;
        private readonly ISubmissionRepository _submissionRepository;

        public CertificateServices(ICloudStorage cloudStorage, ICertificateRepository certificateRepository, IStudentRepository studentRepository, ITrainingProgramRepository trainingProgramRepository, IAttendanceRepository attendanceRepository, IAttendDetaillsRepository attendDetaillsRepository, ISubmissionRepository submissionRepository)
        {
            _cloudStorage = cloudStorage;
            _certificateRepository = certificateRepository;
            _studentRepository = studentRepository;
            _trainingProgramRepository = trainingProgramRepository;
            _attendanceRepository = attendanceRepository;
            _attendDetaillsRepository = attendDetaillsRepository;
            _submissionRepository = submissionRepository;
        }

        public async Task<ResultModel> CertificateFilter(ScoreAssignmentResModel reqModel)
        {
            ResultModel resultModel = new ResultModel();


            return resultModel;

        }
        public async Task<ResultModel> UpdateCertificateStatus(Guid certificateID, string newStatus)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Certificate certificate = await _certificateRepository.GetCertificateById(certificateID);

                if (certificate == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = 400;
                    resultModel.Data = null;

                    return resultModel;
                }

                certificate.Status = newStatus;
                var updatedCertificate = await _certificateRepository.UpdateCertificate(certificate);

                resultModel.IsSuccess = true;
                resultModel.Code = 200;
                resultModel.Data = updatedCertificate;
            }
            catch (Exception e)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = 400;
                resultModel.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return resultModel;
        }

        public async Task<ResultModel> AddCertificate(CertificateModel reqModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                bool programExist = await _certificateRepository.CheckProgramExist(reqModel.programId);

                if (!programExist)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Program with provided ProgramId does not exist.";
                    return result;
                }

                bool studentExist = await _certificateRepository.CheckStudentExist(reqModel.studentId);
                if (!studentExist)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Student with provided StudentId does not exist.";
                    return result;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CertificateModel, Certificate>();
                });
                IMapper mapper = config.CreateMapper();

                Certificate newCertificate = mapper.Map<CertificateModel, Certificate>(reqModel);
                var CertificateId = Guid.NewGuid();

                User student = await _studentRepository.GetStudentById(reqModel.studentId);
                TrainingProgram program = await _trainingProgramRepository.GetTrainingProgramById(reqModel.programId);

                //Upload to Firebase
                string certificateContent = $"This is to certify that {student.FullName} has completed {program.Name} program.";
                string certificateFileName = $"{student.FullName}_{program.Name}.pdf";

                List<string> uploadedFileNames = new List<string>();

                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(certificateContent)))
                {
                    var certificateFile = new MockFormFile(memoryStream, 0, memoryStream.Length, "certificateFile", certificateFileName);

                    string filePath = "certificate";
                    List<IFormFile> certificateFilesList = new List<IFormFile> { certificateFile };
                    uploadedFileNames = await _cloudStorage.UploadFilesToFirebase(certificateFilesList, filePath);
                }
                /////////////

                newCertificate.Id = CertificateId;
                newCertificate.StudentId = reqModel.studentId;
                newCertificate.ProgramId = reqModel.programId;
                newCertificate.Status = CertificateStatus.ACTIVE;

                if (uploadedFileNames.Count > 0)
                {
                    newCertificate.CertificateAttachment = "certificate";
                }

                await _certificateRepository.Insert(newCertificate);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Certificate created successfully!";

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetStudentCertificateList()
        {
            ResultModel Result = new ResultModel();
            try
            {
                var submissions = await _submissionRepository.GetAllSubmission();
                var attendances = await _attendanceRepository.GetAllAttendance();
                var attendanceDetails = await _attendDetaillsRepository.GetAllAttendanceDetails();

                int totalAttendanceSessions = attendances.Count();

                List<IdAndNameModel> eligibleStudents = new List<IdAndNameModel>();

                foreach (var studentId in submissions.Select(s => s.StudentId).Distinct())
                {
                    var passedCertificate = submissions.Where(s => s.Score >= 5 && s.StudentId == studentId).ToList();

                    var attendanceIds = attendances.Select(a => a.Id).ToList();

                    int attendedSessions = attendanceDetails.Count(ad => ad.StudentId == studentId && attendanceIds.Contains(ad.AttendanceId));

                    double attendancePercentage = (double)attendedSessions / totalAttendanceSessions;
                    double attendanceThreshold = 0.8;

                    if (attendancePercentage >= attendanceThreshold && passedCertificate.Any())
                    {
                        var student = await _studentRepository.GetStudentById(studentId);
                        eligibleStudents.Add(new IdAndNameModel { Id = studentId, Name = student.FullName });
                    }
                }

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = eligibleStudents;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 500;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }
    }
}