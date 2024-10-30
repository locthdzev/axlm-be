using Data.Models.CertificateModel;
using Data.Models.ResultModel;
using Data.Models.SubmissionModel;

namespace CertificateAPIServices.Services
{
    public interface ICertificateServices
    {
        Task<ResultModel> CertificateFilter(ScoreAssignmentResModel reqModel);
        Task<ResultModel> UpdateCertificateStatus(Guid certificateID, string newStatus);
        Task<ResultModel> AddCertificate(CertificateModel reqModel);
        Task<ResultModel> GetStudentCertificateList();
    }
}