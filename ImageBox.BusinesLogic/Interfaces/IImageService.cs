using ImageBox.Shared.Interfaces;

namespace ImageBox.BusinessLogic.Interfaces;

public interface IImageService
{
    bool VerifyImageAsync(IFileData fileData);
    Task<(string? imagePath, string? imageHash)> UploadImageAsync(IFileData fileData);
    string GetEncodingFormat();
    bool DeleteImageAsync(string Path);
    Task<byte[]> ImageToByteArray(string filePath);
}