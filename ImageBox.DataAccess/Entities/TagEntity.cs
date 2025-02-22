namespace ImageBox.DataAccess.Entities;

public class TagEntity
{
    public long Id { get; set; }
    public string Tag { get; set; } = string.Empty;
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();
}
