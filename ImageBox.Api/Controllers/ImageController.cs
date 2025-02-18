using ImageBox.Api.DataBase.Entity;
using ImageBox.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImageBox.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ImageController(IImageService imageService, IImageRepository imageRepository) : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult> UploadImage(IFormFile file, long userId)
    {
        bool isImage = await imageService.VerifyImageAsync(file);

        if (!isImage)
            return BadRequest("File is not image.");

        (string imagePath, string imageHash) imageInfo = await imageService.UploadImageAsync(file);

        if (imageInfo.imagePath is null || imageInfo.imageHash is null)
            return BadRequest("Failed to save the image. Please try again later.");

        string imageFormat = imageService.GetEncodedFormat();

        var imageEntity = new ImageEntity
        {
            FileHash = imageInfo.imageHash,
            FilePath = imageInfo.imagePath,
            FileSize = 0,
            FileType = imageFormat,
            UserEntityId = userId
        };

        await imageRepository.CreateAsync(imageEntity);

        return Ok();
    }
}
