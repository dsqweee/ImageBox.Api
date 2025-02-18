using ImageBox.Api.DataBase;
using ImageBox.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.Api.Repositories;

public class Repository<T>(ImageBoxDbContext dbContext) : IRepository<T> where T : class
{
    protected readonly ImageBoxDbContext _dbContext = dbContext;

    public async Task CreateAsync(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long Id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(Id);

        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        var existEntity = await _dbContext.Set<T>().FirstOrDefaultAsync(x=> x == entity);
        _dbContext.Entry<T>(existEntity).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<T> GetByIdAsync(long imageId)
    {
        return await _dbContext.Set<T>().FindAsync(imageId);
    }
}
