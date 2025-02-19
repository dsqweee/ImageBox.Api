using ImageBox.Shared.DTOs.AuthTokenDtos;
using ImageBox.Shared.DTOs.AuthDtos;
using ImageBox.DataAccess.Entities;

namespace ImageBox.BusinessLogic.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(UserAuthDto request);
    Task<UserEntity?> RegisterAsync(UserAuthDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
}