using Amazon.S3;
using Amazon.S3.Model;
using FileSignatures;
using ImageBox.BusinessLogic.Interfaces;
using ImageBox.Shared.Config;
using ImageBox.Shared.Interfaces;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Net;

namespace ImageBox.BusinessLogic.Services;

public class ImageS3Service : IImageS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3Settings> _s3settings;
    private readonly IFileFormatInspector _fileFormatInspector;

    public ImageS3Service(IAmazonS3 s3Client, IOptions<S3Settings> s3settings, IFileFormatInspector fileFormatInspector)
    {
        _s3Client = s3Client;
        _s3settings = s3settings;
        _fileFormatInspector = fileFormatInspector;
    }

    private const int desiredWidth = 1920;
    private const int desiredHeight = 1080;

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

    private void ResizeProportionally(ref int originalWidth, ref int originalHeight)
    {
        double scale;

        if (originalWidth > originalHeight)
        {
            scale = (double)desiredWidth / originalWidth;

            originalWidth = desiredWidth;
            originalHeight = (int) (originalHeight * scale);
        }
        else
        {
            scale = (double)desiredHeight / originalHeight;

            originalWidth = (int) (originalWidth * scale);
            originalHeight = desiredHeight;
        }
    }

    private void MutateImage(Image image)
    {
        int width = image.Width;
        int height = image.Height;

        if (image.Width > 1920 || image.Height > 1080)
        {
            ResizeProportionally(ref width, ref height);
            image.Mutate(x => x.Resize(width, height));
        }
    }

    //private async Task<MemoryStream> SaveImageToStreamAsync(IFileData file)
    //{
    //    using var memoryStream = new MemoryStream();
    //    using var image = Image.Load(file.OpenReadStream());

    //    MutateImage(image);

    //    await image.SaveAsync(memoryStream, EncodeSettings);
    //    return memoryStream;
    //}


    public async Task<string?> UploadFileAsync(IFileData file)
    {
        var key = Guid.NewGuid().ToString();
        var encodeFormat = GetEncodingFormat();

        //using var imageStream = await SaveImageToStreamAsync(file);

        using var memoryStream = new MemoryStream();
        using var image = Image.Load(file.OpenReadStream());

        MutateImage(image);

        await image.SaveAsync(memoryStream, EncodeSettings);

        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _s3settings.Value.BucketName,
                Key = $"{key}.{encodeFormat}",
                InputStream = memoryStream
            };
            var response = await _s3Client.PutObjectAsync(putRequest);
        }
        catch (Exception)
        {
            return null;
        }
        
        return key;
    }

    public string GetImageLinkById(string id)
    {
        var encodeFormat = GetEncodingFormat();
        return $"{_s3settings.Value.Endpoint}/{_s3settings.Value.BucketName}/{id}.{encodeFormat}";
    }

    public async Task<HttpStatusCode> DeleteImageByIdAsync(string id)
    {
        var encodeFormat = GetEncodingFormat();

        try
        {
            var putRequest = new DeleteObjectRequest
            {
                BucketName = _s3settings.Value.BucketName,
                Key = $"{id}.{encodeFormat}"
            };

            var response = await _s3Client.DeleteObjectAsync(putRequest);
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
        FileFormat? format = _fileFormatInspector.DetermineFileFormat(fileStream);

        return format is FileSignatures.Formats.Image;
    }
}
