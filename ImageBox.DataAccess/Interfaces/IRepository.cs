namespace ImageBox.DataAccess.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(long Id);
        Task<T> GetByIdAsync(long Id);
        Task UpdateAsync(T entity);
    }
}