using ImageBox.Api.DataBase;
using ImageBox.Api.DataBase.Entity;
using ImageBox.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.Api.Repositories;

public class UserRepository : Repository<UserEntity>, IUserRepository 
{
    public UserRepository(ImageBoxDbContext dbContext) : base(dbContext)
    {
        
    }

    //public async Task CreateUserAsync(UserEntity user)
    //{
    //    await _dbContext.users.AddAsync(user);
    //    await _dbContext.SaveChangesAsync();
    //}

    //public async Task DeleteUserAsync(long userId)
    //{
    //    var userEntity = new UserEntity { Id = userId };

    //    _dbContext.users.Remove(userEntity);
    //    await _dbContext.SaveChangesAsync();
    //}

    public async Task<bool> GetExistUserByUsernameAsync(string username)
    {
        return await _dbContext.users.AnyAsync(x=>x.Username == username);
    }

    public async Task<UserEntity> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.users.FirstOrDefaultAsync(x=>x.Username == username);
    }

    //public async Task UpdateUserAsync(UserEntity user)
    //{
    //    var userInDb = await _dbContext.users.FindAsync(user.Id);

    //    _dbContext.Entry<UserEntity>(userInDb).CurrentValues.SetValues(user);
    //    await _dbContext.SaveChangesAsync();
    //}

    //public async Task<UserEntity> GetUserByIdAsync(long userId)
    //{
    //    return await _dbContext.users.FindAsync(userId);
    //}
}
