using FileSignatures;
using ImageBox.BusinessLogic.Interfaces;
using ImageBox.Shared.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;

namespace ImageBox.BusinessLogic.Services;

public class ImageFileService(IFileFormatInspector fileFormatInspector) : IImageFileService
{
    private readonly string DirectoryImage = "storage";
    private readonly IImageFormat EncodeFormat = JpegFormat.Instance;
    private readonly ImageEncoder EncodeSettings = new JpegEncoder()
    {
        SkipMetadata = true,
        Quality = 85
    };

    public string GetEncodingFormat()
    {
        return EncodeFormat.Name.ToLower();
    }

    
    public async Task<(string? imagePath, string? imageHash)> CreateImageFileAsync(IFileData fileData)
    {
        string fileHash = GenerateRandomHash();

        string filePath = GenerateFilePath(fileHash);

        bool saveStatus = await SaveImageStorageAsync(fileData, filePath);

        if (!saveStatus)
            return (null, null);

        return (filePath, fileHash);
    }

    public bool DeleteImageFileAsync(string filePath)
    {
        try
        {
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
                file.Delete();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }


    public async Task<byte[]> GetImageFileAsync(string filePath)
    {
        using var imageFile = await Image.LoadAsync(filePath);
        using var ms = new MemoryStream();

        imageFile.Save(ms, EncodeFormat);
        return ms.ToArray();
    }


    public bool VerifyImageAsync(IFileData fileData)
    {
        using var fileStream = fileData.OpenReadStream();
        FileFormat? format = fileFormatInspector.DetermineFileFormat(fileStream);

        if (format is not FileSignatures.Formats.Image)
            return false;

        return true;
    }





    private string GenerateRandomHash()
    {
        using var rng = RandomNumberGenerator.Create();

        var hashBytes = new byte[32];
        rng.GetBytes(hashBytes);

        string hashedString = BitConverter.ToString(hashBytes)
                                          .Replace("-", "")
                                          .ToLower();

        return hashedString;
    }

    private string GenerateFilePath(string fileHash)
    {
        var fileFormat = GetEncodingFormat();
        var directory = Path.Combine(fileHash[0..2], fileHash[2..4], fileHash[4..6]);
        var fullPath = Path.Combine(DirectoryImage, directory, $"{fileHash}.{fileFormat}");

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        return fullPath;
    }


    private async Task<bool> SaveImageStorageAsync(IFileData fileData, string filePath)
    {
        using var fileStream = fileData.OpenReadStream();
        using var image = Image.Load(fileStream);

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
