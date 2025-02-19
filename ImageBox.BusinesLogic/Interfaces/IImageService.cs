namespace ImageBox.BusinessLogic.Interfaces;

public interface IImageService
{
    Task<bool> VerifyImageAsync(MemoryStream memoryStream);
    Task<(string imagePath, string imageHash)> UploadImageAsync(MemoryStream memoryStream);
    string GenerateRandomHash();
    string GetEncodedFormat();
    bool DeleteImageAsync(string Path);
    Task<byte[]> ImageToByteArray(string filePath);
}