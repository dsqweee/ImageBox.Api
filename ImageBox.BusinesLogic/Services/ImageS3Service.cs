using Amazon.S3;
using Amazon.S3.Model;
using FileSignatures;
using ImageBox.BusinessLogic.Interfaces;
using ImageBox.Shared.Config;
using ImageBox.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Net;

namespace ImageBox.BusinessLogic.Services;

public class ImageS3Service(IAmazonS3 s3client, IOptions<S3Settings> s3settings, IFileFormatInspector fileFormatInspector) : IImageS3Service
{
    private readonly IImageFormat EncodeFormat = WebpFormat.Instance;
    private readonly ImageEncoder EncodeSettings = new WebpEncoder()
    {
        SkipMetadata = true,
        Quality = 75, // Качество (1-100)
        Method = WebpEncodingMethod.Level0,   // Метод сжатия (0-6)
        UseAlphaCompression = true, // Сжатие альфа-канала
        EntropyPasses = 2, // Количество проходов для оптимизации энтропии
        FilterStrength = 60, // Сила фильтрации
        NearLossless = false, // Включить режим "почти без потерь"
        NearLosslessQuality = 75
    };

    public string GetEncodingFormat()
    {
        return EncodeFormat.Name.ToLower();
    }

    private (int newWidth, int newHeight) ResizeProportionally(int originalWidth, int originalHeight)
    {
        int desiredWidth = 1920;
        int desiredHeight = 1080;

        double scale;

        if (originalWidth > originalHeight)
        {
            scale = (double)desiredWidth / originalWidth;
            return (desiredWidth, (int)(originalHeight * scale));
        }
        else
        {
            scale = (double)desiredHeight / originalHeight;
            return ((int)(originalWidth * scale), desiredHeight);
        }
    }


    public async Task<string> UploadFileAsync(IFileData file)
    {
        var key = Guid.NewGuid().ToString();
        var encodeFormat = GetEncodingFormat();

        using var memoryStream = new MemoryStream();
        using var image = Image.Load(file.OpenReadStream());

        if(image.Width > 1920 || image.Height > 1080)
        {
            (int newWidth, int newHeight) sizes = ResizeProportionally(image.Width, image.Height);
            image.Mutate(x => x.Resize(sizes.newWidth, sizes.newHeight));
        }

        await image.SaveAsync(memoryStream, EncodeSettings);

        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = s3settings.Value.BucketName,
                Key = $"{key}.{encodeFormat}",
                InputStream = memoryStream
            };
            var response = await s3client.PutObjectAsync(putRequest);
        }
        catch (Exception)
        {
            return null;
        }
        
        return key;
    }

    public async Task<string> GetImageLinkByIdAsync(string id)
    {
        var encodeFormat = GetEncodingFormat();
        return $"{s3settings.Value.Endpoint}/{s3settings.Value.BucketName}/{id}.{encodeFormat}";
    }

    public async Task<HttpStatusCode> DeleteImageByIdAsync(string id)
    {
        var encodeFormat = GetEncodingFormat();

        try
        {
            var putRequest = new DeleteObjectRequest
            {
                BucketName = s3settings.Value.BucketName,
                Key = $"{id}.{encodeFormat}"
            };

            var response = await s3client.DeleteObjectAsync(putRequest);
        }
        catch (Exception)
        {
            return HttpStatusCode.BadRequest;
        }

        return HttpStatusCode.OK;
    }


    public bool VerifyImageAsync(IFileData fileData)
    {
        using var fileStream = fileData.OpenReadStream();
        FileFormat? format = fileFormatInspector.DetermineFileFormat(fileStream);

        if (format is not FileSignatures.Formats.Image)
            return false;

        return true;
    }
}
