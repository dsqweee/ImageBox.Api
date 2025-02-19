using ImageBox.BusinessLogic.Interfaces;
using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImageBox.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ImageController(IImageService imageService, IImageRepository imageRepository) : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult> UploadImage(IFormFile file, long userId)
    {
        using (var stream = file.OpenReadStream())
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0; 

            bool isImage = await imageService.VerifyImageAsync(memoryStream);

            if (!isImage)
                return BadRequest("File is not image.");

            (string imagePath, string imageHash) imageInfo = await imageService.UploadImageAsync(memoryStream);

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

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteImage(long id)
    {
        var existImage = await imageRepository.GetByIdAsync(id);
        if (existImage is null)
            return BadRequest("Image is not found.");

        bool deleteStatus = imageService.DeleteImageAsync(existImage.FilePath);

        if (!deleteStatus)
            return BadRequest("Failed to delete the image. Please try again later.");

        await imageRepository.DeleteAsync(id);

        return Ok();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult> GetImage(long id)
    {
        var imageDb = await imageRepository.GetByIdAsync(id);
        if (imageDb is null)
            return BadRequest("Image is not found.");

        var imageBytes = await imageService.ImageToByteArray(imageDb.FilePath);

        return File(imageBytes, "image/jpeg");
    }
}
