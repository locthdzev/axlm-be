using Data.Entities;
using Data.Models.ClassModel;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.ClassTrainerRepositories
{
    public class ClassTrainerRepository : Repository<ClassTrainer>, IClassTrainerRepository
    {
        private readonly AXLMDbContext _context;

        public ClassTrainerRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ClassTrainer?> GetClassTrainerByClassId(Guid classId)
        {
            return await _context.ClassTrainers.Where(ct => ct.ClassId == classId).FirstOrDefaultAsync();
        }

        public async Task<List<NoneTrainerClassListModel>> GetNoneTrainerClasses()
        {
            var allClass = await _context.Classes.Select(c => c.Id).ToListAsync();
            var trainClass = await _context.ClassTrainers.Select(c => c.ClassId).ToListAsync();
            var selected = allClass.Except(trainClass);

            return await _context.Classes.Where(c => selected.Contains(c.Id))
                                       .Join(_context.ClassManagers, c => c.Id, cm => cm.ClassId, (c, cm) => new NoneTrainerClassListModel
                                       {
                                           ClassId = c.Id,
                                           ClassName = c.Name,
                                           AdminId = cm.User.Id,
                                           AdminName = cm.User.FullName,
                                           AdminEmail = cm.User.Email
                                       })
                                       .ToListAsync();

        }

        public async Task<List<Guid>> GetListClassIdByTrainerId(Guid trainerId)
        {
            return await _context.ClassTrainers.Where(ct => ct.UserId == trainerId).Select(ct => ct.ClassId).ToListAsync();
        }
    }
}