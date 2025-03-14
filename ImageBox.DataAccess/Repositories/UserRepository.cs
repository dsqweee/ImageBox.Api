using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class UserRepository : /*Repository<UserEntity>(dbContext),*/ IUserRepository 
{

    private readonly ImageBoxDbContext _dbContext;
    public UserRepository(ImageBoxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateUserAsync(UserEntity entity)
    {
        await _dbContext.users.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(long Id)
    {
        var entity = await _dbContext.users.FindAsync(Id);

        _dbContext.users.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(UserEntity entity)
    {
        var existEntity = await _dbContext.users.FirstOrDefaultAsync(x => x.Id == entity.Id);

        _dbContext.Entry<UserEntity>(existEntity).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<UserEntity?> GetUserByIdAsync(long Id)
    {
        return await _dbContext.users.FindAsync(Id);
    }


    public async Task<bool> GetExistUserByUsernameAsync(string username)
    {
        return await _dbContext.users.AnyAsync(x=>x.Username == username);
    }

    public async Task<UserEntity?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.users.FirstOrDefaultAsync(x=>x.Username == username);
    }
}
