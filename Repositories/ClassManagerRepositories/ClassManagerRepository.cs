using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.ClassManagerRepositories
{
    public class ClassManagerRepository : Repository<ClassManager>, IClassManagerRepository
    {
        private readonly AXLMDbContext _context;

        public ClassManagerRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ClassManager?> GetClassManagerByClassId(Guid classId)
        {
            return await _context.ClassManagers.Where(cm => cm.ClassId == classId).FirstOrDefaultAsync();
        }

        public async Task<List<Guid>> GetListClassIdByManagerId(Guid managerId)
        {
            return await _context.ClassManagers.Where(cm => cm.UserId == managerId).Select(cm => cm.ClassId).ToListAsync();
        }
    }
}