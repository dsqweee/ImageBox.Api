namespace ImageBox.Api.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(long Id);
        Task<T> GetByIdAsync(long imageId);
        Task UpdateAsync(T entity);
    }
}