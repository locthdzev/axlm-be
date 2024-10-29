using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.DocumentRepositories
{
    public class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        private readonly AXLMDbContext _context;

        public DocumentRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Document>> GetAll()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<List<Document>?> GetListDocByModuleId(Guid moduleId)
        {
            return await _context.Lectures.Where(l => l.ModuleId == moduleId)
                .Join(_context.Documents, l => l.Id, d => d.LectureId, (l, d) => d)
                .ToListAsync();
        }

        public async Task<List<Document>> GetListDocumentByLectureId(Guid Id)
        {
            return await _context.Documents.Where(x => x.LectureId.Equals(Id)).ToListAsync();
        }
    }
}