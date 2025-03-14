namespace ImageBox.Shared.DTOs.ImageDtos;

public class UploadImageDto
{
    public long UserId { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}
