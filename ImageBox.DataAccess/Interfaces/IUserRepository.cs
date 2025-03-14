using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface IUserRepository //: IRepository<UserEntity>
{
    Task CreateUserAsync(UserEntity entity);
    Task DeleteUserAsync(long Id);
    Task<UserEntity?> GetUserByIdAsync(long Id);
    Task UpdateUserAsync(UserEntity entity);

    Task<bool> GetExistUserByUsernameAsync(string username);
    Task<UserEntity?> GetUserByUsernameAsync(string username);
}
