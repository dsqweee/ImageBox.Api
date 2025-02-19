namespace ImageBox.Shared.DTOs.AuthTokenDtos;

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
