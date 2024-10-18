namespace Data.GenericRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(Guid id);
        Task Insert(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        Task AddRange(List<T> entities);
        Task DeleteRange(List<T> entities);
        Task UpdateRange(List<T> entities);
    }
}