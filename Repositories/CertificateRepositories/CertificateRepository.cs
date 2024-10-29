using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.CertificateRepositories
{
    public class CertificateRepository :  Repository<Certificate>, ICertificateRepository
    {
        private readonly AXLMDbContext _context;

        public CertificateRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckProgramExist(Guid programId)
        {
            return await _context.TrainingPrograms.AnyAsync(p => p.Id == programId);
        }

        public async Task<bool> CheckStudentExist(Guid studentId)
        {
            return await _context.Users.AnyAsync(u => u.Id == studentId);
        }

        public async Task<Certificate?> GetCertificateById(Guid certificateId)
        {
            var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.Id == certificateId);
            return certificate;
        }

        public async Task<Certificate?> UpdateCertificate(Certificate certificate)
        {
            var certificateUpdate = await _context.Certificates.FirstOrDefaultAsync(c => c.Id == certificate.Id);

            if (certificateUpdate is null)
            {
                return certificate;
            }

            certificateUpdate.StudentId = certificate.StudentId;
            certificateUpdate.ProgramId = certificate.ProgramId;
            certificateUpdate.CertificateAttachment = certificate.CertificateAttachment;
            certificateUpdate.Status = certificate.Status;

            await _context.SaveChangesAsync();
            return certificateUpdate;
        }
    }
}