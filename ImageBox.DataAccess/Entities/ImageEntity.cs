namespace ImageBox.DataAccess.Entities;

public class ImageEntity
{
    public long Id { get; set; }
    public string FileType { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long UserEntityId { get; set; }
    public UserEntity UserEntity { get; set; }

    public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();
}