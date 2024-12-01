using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.GenericRepositories
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
            await _entities.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task Remove(T entity)
        {
            _entities.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _entities.Update(entity);
            await SaveChangesAsync();
        }

        public async Task AddRange(List<T> entities)
        {
            _entities.AddRange(entities);
            await SaveChangesAsync();
        }

        public async Task DeleteRange(List<T> entities)
        {
            _entities.RemoveRange(entities);
            await SaveChangesAsync();
        }

        public async Task UpdateRange(List<T> entities)
        {
            _entities.UpdateRange(entities);
            await SaveChangesAsync();
        }

        // Phương thức riêng để gọi SaveChangesAsync, giúp tối ưu hóa và giảm số lần gọi.
        private async Task SaveChangesAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }
    }
}
