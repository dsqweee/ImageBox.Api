namespace ImageBox.Api.DataBase.Entity;

public class UserEntity
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<ImageEntity> ImageEntities { get; set; } = new List<ImageEntity>();
}
