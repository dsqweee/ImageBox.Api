using ImageBox.BusinessLogic.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;

namespace ImageBox.BusinessLogic.Services;

public class ImageService : IImageService
{
    private readonly string DirectoryImage = "storage";
    private readonly JpegFormat EncodeFormat = JpegFormat.Instance;
    private readonly JpegEncoder EncodeSettings = new JpegEncoder()
    {
        SkipMetadata = true,
        Quality = 85
    };

    public string GetEncodedFormat()
    {
        return EncodeFormat.Name.ToLower();
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
        var fileFormat = GetEncodedFormat();
        var directory = Path.Combine(fileHash[0..2], fileHash[2..4], fileHash[4..6]);
        var fullPath = Path.Combine(DirectoryImage, directory, $"{fileHash}.{fileFormat}");

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        return fullPath;
    }

    public async Task<bool> VerifyImageAsync(MemoryStream memoryStream)
    {
        using (memoryStream)
        {
            try
            {
                await Image.DetectFormatAsync(memoryStream);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }


    private async Task<bool> SaveImageToStorageAsync(MemoryStream memoryStream, string filePath)
    {
        using (var image = Image.Load(memoryStream))
        {
            try
            {
                await image.SaveAsync(filePath, EncodeSettings);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }

    public async Task<(string imagePath, string imageHash)> UploadImageAsync(MemoryStream memoryStream)
    {
        using (memoryStream)
        {
            string fileHash = GenerateRandomHash();

            string filePath = GenerateFilePath(fileHash);

            bool saveStatus = await SaveImageToStorageAsync(memoryStream, filePath);

            if (!saveStatus)
                return (null, null);

            return (filePath, fileHash);
        }
    }

    public bool DeleteImageAsync(string Path)
    {
        try
        {
            FileInfo file = new FileInfo(Path);
            if (file.Exists)
                file.Delete();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public async Task<byte[]> ImageToByteArray(string filePath)
    {
        using (var imageFile = await Image.LoadAsync(filePath))
        using (MemoryStream ms = new MemoryStream())
        {
            imageFile.Save(ms, EncodeFormat);
            return ms.ToArray();
        }
    }
}
