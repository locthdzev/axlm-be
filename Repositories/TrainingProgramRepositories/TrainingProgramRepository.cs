using Data.Entities;
using Data.Models.TrainingProgramModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;
using static Data.Enums.Status;

namespace Repositories.TrainingProgramRepositories
{
    public class TrainingProgramRepository : Repository<TrainingProgram>, ITrainingProgramRepository
    {
        private readonly AXLMDbContext _context;

        public TrainingProgramRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TrainingProgram?> GetTrainingProgramById(Guid TrainingProgramId)

        {
            return await _context.TrainingPrograms
                .FirstOrDefaultAsync(t => t.Id == TrainingProgramId);
        }
        public async Task<TrainingProgram?> GetTrainingProgramByName(string TrainingProgramName)
        {
            return await _context.TrainingPrograms
                .FirstOrDefaultAsync(t => t.Name == TrainingProgramName);
        }

        public async Task<List<TrainingProgram>> GetListProgramTraining()
        {
            return await _context.TrainingPrograms.Where(x => x.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<List<TrainingProgram>> GetListProgramTrainingByListId(List<Guid> ListId)
        {
            return await _context.TrainingPrograms.Where(x => ListId.Contains(x.Id)).ToListAsync();
        }

        public async Task<List<TrainingProgramDropDownResModel>> GetListDropDownProgramTraining()
        {
            return await _context.TrainingPrograms.Where(x => x.Status.Equals(GeneralStatus.ACTIVE)).Select(x => new TrainingProgramDropDownResModel()
            {
                Id = x.Id,
                Code = x.Code
            }).ToListAsync();
        }
    }
}