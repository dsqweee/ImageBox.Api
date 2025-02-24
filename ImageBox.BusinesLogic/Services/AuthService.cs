using ImageBox.BusinessLogic.Interfaces;
using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using ImageBox.Shared.DTOs.AuthDtos;
using ImageBox.Shared.DTOs.AuthTokenDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ImageBox.BusinessLogic.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
{
    public async Task<TokenResponseDto?> LoginAsync(UserAuthDto request)
    {
        var user = await userRepository.GetUserByUsernameAsync(request.Username);
        if (user is null)
        {
            return null;
        }

        var passwordHasher = new PasswordHasher<UserEntity>();
        var verifyResult = passwordHasher
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await CreateTokenResponse(user); 
    }

    public async Task<UserEntity?> RegisterAsync(UserAuthDto request)
    {
        var userExist = await userRepository.GetExistUserByUsernameAsync(request.Username);
        if (userExist)
        {
            return null;
        }

        var user = new UserEntity();
        var hashedPassword = new PasswordHasher<UserEntity>()
            .HashPassword(user, request.Password);

        user.Username = request.Username;
        user.PasswordHash = hashedPassword;

        await userRepository.CreateAsync(user);

        return user;
    }


    private async Task<TokenResponseDto> CreateTokenResponse(UserEntity? user)
    {
        var accessToken = CreateToken(user);
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);

        var response = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return response;
    }

    private string CreateToken(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };


        var configKey = configuration["Jwt:Key"]!;
        var bytesKey = Encoding.UTF8.GetBytes(configKey);

        var key = new SymmetricSecurityKey(bytesKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);


        var configIssuer = configuration["Jwt:Issuer"]!;
        var configAudience = configuration["Jwt:Audience"]!;
        var expires = DateTime.UtcNow.AddDays(1);

        var tokenDescriptor = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds,
            issuer: configIssuer,
            audience: configAudience,
            expires: expires
            );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(UserEntity user)
    {
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await userRepository.UpdateAsync(user);

        return refreshToken;
    }

    public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (user is null)
        {
            return null;
        }

        return await CreateTokenResponse(user);
    }

    private async Task<UserEntity?> ValidateRefreshTokenAsync(long userId, string refreshToken)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null ||
            user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }
}
