using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<bool> GetExistUserByUsernameAsync(string username);
    Task<UserEntity?> GetUserByUsernameAsync(string username);
}
