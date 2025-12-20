namespace ERP.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity, int createdBy);
        Task<bool> UpdateAsync(T entity, int updatedBy);
        Task<bool> SoftDeleteAsync(int id, int updatedBy);
        IQueryable<T> Query();
    }
}
