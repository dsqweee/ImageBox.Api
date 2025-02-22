using FileSignatures;
using ImageBox.BusinessLogic.Interfaces;
using ImageBox.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Security.Cryptography;

namespace ImageBox.BusinessLogic.Services;

public class ImageService(IFileFormatInspector fileFormatInspector) : IImageService
{
    private readonly string DirectoryImage = "storage";
    private readonly JpegFormat EncodeFormat = JpegFormat.Instance;
    private readonly JpegEncoder EncodeSettings = new JpegEncoder()
    {
        SkipMetadata = true,
        Quality = 85
    };

    public string GetEncodingFormat()
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
        var fileFormat = GetEncodingFormat();
        var directory = Path.Combine(fileHash[0..2], fileHash[2..4], fileHash[4..6]);
        var fullPath = Path.Combine(DirectoryImage, directory, $"{fileHash}.{fileFormat}");

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        return fullPath;
    }


    public bool VerifyImageAsync(IFileData fileData)
    {
        using var fileStream = fileData.OpenReadStream();
        FileFormat? format = fileFormatInspector.DetermineFileFormat(fileStream);

        if(format is not FileSignatures.Formats.Image)
            return false;

        return true;
    }


    private async Task<bool> SaveImageToStorageAsync(IFileData fileData, string filePath)
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

    public async Task<(string? imagePath, string? imageHash)> UploadImageAsync(IFileData file)
    {
        string fileHash = GenerateRandomHash();

        string filePath = GenerateFilePath(fileHash);

        bool saveStatus = await SaveImageToStorageAsync(file, filePath);

        if (!saveStatus)
            return (null, null);

        return (filePath, fileHash);
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
