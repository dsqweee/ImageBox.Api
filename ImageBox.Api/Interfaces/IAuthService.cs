using ImageBox.Api.DataBase.Entity;
using ImageBox.Api.DTOs.AuthDtos;
using ImageBox.Api.DTOs.AuthTokenDtos;

namespace ImageBox.Api.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(UserAuthDto request);
    Task<UserEntity?> RegisterAsync(UserAuthDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
}