using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class UserRepository : Repository<UserEntity>, IUserRepository 
{
    public UserRepository(ImageBoxDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<bool> GetExistUserByUsernameAsync(string username)
    {
        return await _dbContext.users.AnyAsync(x=>x.Username == username);
    }

    public async Task<UserEntity> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.users.FirstOrDefaultAsync(x=>x.Username == username);
    }
}
