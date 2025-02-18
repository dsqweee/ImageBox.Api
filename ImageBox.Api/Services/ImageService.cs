using ImageBox.Api.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;

namespace ImageBox.Api.Services;

public class ImageService : IImageService
{
    private readonly string DirectoryImage = "storage";
    private readonly string EncodeFormat = JpegFormat.Instance.Name.ToLower();
    private readonly JpegEncoder EncodeSettings = new JpegEncoder()
    {
        SkipMetadata = true,
        Quality = 85
    };

    public string GetEncodedFormat()
    {
        return EncodeFormat;
    }

    public string GenerateRandomHash()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var hashBytes = new byte[32];
            rng.GetBytes(hashBytes);

            string hashedString = BitConverter.ToString(hashBytes)
                                              .Replace("-", "")
                                              .ToLower();

            return hashedString;
        }
    }

    private string GenerateFilePath(string fileHash)
    {
        var directory = Path.Combine(fileHash[0..2], fileHash[2..4], fileHash[4..6]);
        var fullPath = Path.Combine(DirectoryImage, directory, $"{fileHash}.{EncodeFormat}");

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        return fullPath;
    }

    public async Task<bool> VerifyImageAsync(IFormFile file)
    {
        using (var Stream = file.OpenReadStream())
        {
            try
            {
                await Image.IdentifyAsync(Stream);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }


    private async Task<bool> SaveImageToStorageAsync(IFormFile file, string filePath)
    {
        using (var Stream = file.OpenReadStream())
        using (var image = Image.Load(Stream))
        {
            try
            {
                await image.SaveAsync(filePath, EncodeSettings);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }

    public async Task<(string imagePath, string imageHash)> UploadImageAsync(IFormFile file)
    {
        string fileHash = GenerateRandomHash();

        string filePath = GenerateFilePath(fileHash);

        bool saveStatus = await SaveImageToStorageAsync(file, filePath);

        if (!saveStatus)
            return (null, null);
        
        return (filePath, fileHash);
    }
}
