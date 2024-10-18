using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.GenericRepositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AXLMDbContext context;
        private readonly DbSet<T> _entities;

        public Repository(AXLMDbContext context)
        {
            this.context = context;
            _entities = context.Set<T>();
        }

        public async Task<T?> Get(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Insert(T entity)
        {
            _ = await _entities.AddAsync(entity);
            _ = await context.SaveChangesAsync();
        }

        public async Task Remove(T entity)
        {
            _ = _entities.Remove(entity);
            _ = await context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _ = _entities.Update(entity);
            _ = await context.SaveChangesAsync();
        }

        public async Task AddRange(List<T> entities)
        {
            _entities.AddRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRange(List<T> entities)
        {
            _entities.RemoveRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRange(List<T> entities)
        {
            _entities.UpdateRange(entities);
            await context.SaveChangesAsync();
        }
    }
}