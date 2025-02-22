using ImageBox.BusinessLogic.Interfaces;
using ImageBox.DataAccess.Entities;
using ImageBox.Shared.DTOs.AuthDtos;
using ImageBox.Shared.DTOs.AuthTokenDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageBox.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserEntity>> Register(UserAuthDto request)
    {
        var user = await authService.RegisterAsync(request);

        if (user is null)
            return BadRequest("Username already exists.");

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserAuthDto request)
    {
        var resultResponse = await authService.LoginAsync(request);

        if (resultResponse is null)
            return BadRequest("Username or Password is wrong.");

        return Ok(resultResponse);
    }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are Authorization!");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnlyEndpoint()
    {
        return Ok("You are Admin!");
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<ActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshTokensAsync(request);
        if (result is null || result.AccessToken is null || result.RefreshToken is null)
            return BadRequest("Invalid refresh token.");

        return Ok(result);
    }
}
