using Data.Entities;
using Data.Models.ModuleModel;
using Data.Models.ModuleProgramModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.ModuleProgramRepositories
{
    public class ModuleProgramRepository : Repository<ModuleProgram>, IModuleProgramRepository
    {
        private readonly AXLMDbContext _context;

        public ModuleProgramRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ModuleProgramReqModel>> GetModulesByProgramId(Guid programId)
        {
            return await _context.ModulePrograms.Where(mp => mp.ProgramId == programId && mp.Status.Equals(GeneralStatus.ACTIVE))
                .Include(m => m.Module)
                .OrderByDescending(mp => mp.Module.CreatedAt)
                .Select(mp => new ModuleProgramReqModel
                {
                    Module = mp.Module,
                    TrainingProgram = mp.Program
                })
                .ToListAsync();
        }

        public async Task<List<OtherModuleListRes>> GetModuleOfOtherProgram(Guid programId)
        {
            var thisListId = await _context.ModulePrograms.Where(m => m.ProgramId == programId).Select(m => m.ModuleId).ToListAsync();
            var all = await _context.Modules.Where(m => m.Status.Equals(GeneralStatus.ACTIVE)).Select(m => m.Id).ToListAsync();

            var otherId = all.Except(thisListId).ToList();
            return await _context.Modules.Where(m => otherId.Contains(m.Id))
                                .OrderByDescending(m => m.CreatedAt)
                                .Select(m => new OtherModuleListRes
                                {
                                    Id = m.Id,
                                    Name = m.Name,
                                    Code = m.Code,
                                    numOfProgram = _context.ModulePrograms.Count(mp => mp.ModuleId == m.Id)
                                }).ToListAsync();
        }

        public async Task<List<ModuleProgram>> GetModuleProgramByProgramId(Guid programId)
        {
            return await _context.ModulePrograms.Where(mp => mp.ProgramId == programId).ToListAsync();
        }

        public async Task<List<ModuleProgram>> GetModuleProgramByModuleId(Guid moduleId)
        {
            return await _context.ModulePrograms.Where(mp => mp.ModuleId == moduleId).ToListAsync();
        }
    }
}