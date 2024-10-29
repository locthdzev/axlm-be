using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.DocumentRepositories
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<List<Document>> GetAll();
        Task<List<Document>> GetListDocByModuleId(Guid moduleId);
        Task<List<Document>> GetListDocumentByLectureId(Guid Id);
    }
}