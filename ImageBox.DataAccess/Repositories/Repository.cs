using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class Repository<T>(ImageBoxDbContext dbContext) : IRepository<T> where T : class
{
    protected readonly ImageBoxDbContext _dbContext = dbContext;

    public async Task CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
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
        var existEntity = await _dbContext.Set<T>().FirstOrDefaultAsync(x=> x == entity); // existEntity будет null, если в entity не будет содержаться Id
        _dbContext.Entry<T>(existEntity).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<T?> GetByIdAsync(long Id)
    {
        return await _dbContext.Set<T>().FindAsync(Id);
    }
}
