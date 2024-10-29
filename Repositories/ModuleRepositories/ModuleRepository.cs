using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.ModuleRepositories
{
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        private readonly AXLMDbContext _context;

        public ModuleRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Module>> GetModule()
        {
            return await _context.Modules.Where(m => m.Status.Equals(GeneralStatus.ACTIVE))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Module>> GetListModuleById(List<Guid> moduleid)
        {
            return await _context.Modules.Where(m => moduleid.Contains(m.Id)).ToListAsync();
        }

        public async Task<Module?> GetModuleById(Guid moduleid)
        {
            return await _context.Modules.Where(m => m.Id == moduleid).FirstOrDefaultAsync();
        }

        public async Task<Module?> GetModuleByName(string name)
        {
            return await _context.Modules
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<List<Module>> GetAllModules()
        {
            return await _context.Modules.Where(m => m.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<List<Module>> GetModuleByProgramId(Guid programId)
        {
            var ListModuleProgram = await _context.ModulePrograms.Where(mp => mp.ProgramId == programId).Select(x => x.ModuleId).ToListAsync();
            return await _context.Modules.Where(m => ListModuleProgram.Contains(m.Id)).ToListAsync();
        }
    }
}