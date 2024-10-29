using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.CertificateRepositories
{
    public interface ICertificateRepository : IRepository<Certificate>
    {
        Task<Certificate?> GetCertificateById(Guid certificateId);
        Task<Certificate?> UpdateCertificate(Certificate certificate);
        Task<bool> CheckProgramExist(Guid programId);
        Task<bool> CheckStudentExist(Guid studentId);
    }
}