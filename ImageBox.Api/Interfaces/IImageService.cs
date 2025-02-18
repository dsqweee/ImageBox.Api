namespace ImageBox.Api.Interfaces;

public interface IImageService
{
    Task<bool> VerifyImageAsync(IFormFile file);
    Task<(string imagePath, string imageHash)> UploadImageAsync(IFormFile file);
    string GenerateRandomHash();
    string GetEncodedFormat();
}