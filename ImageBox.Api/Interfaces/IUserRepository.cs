using ImageBox.Api.DataBase.Entity;

namespace ImageBox.Api.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    //Task CreateUserAsync(UserEntity user);
    //Task UpdateUserAsync(UserEntity user);
    //Task DeleteUserAsync(long userId);
    Task<bool> GetExistUserByUsernameAsync(string username);

    Task<UserEntity> GetUserByUsernameAsync(string username);
    //Task<UserEntity> GetUserByIdAsync(long userId);
}
