using ImageBox.BusinessLogic.Interfaces;
using ImageBox.DataAccess.Interfaces;
using ImageBox.Shared.DTOs.ImageDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageBox.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ImageController(IImageS3Service imageService, IImageRepository imageRepository, ITagService tagService, IUserRepository userRepository) : ControllerBase
{

    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)]
    [Authorize]
    public async Task<ActionResult> UploadImage(IFormFile file, [FromQuery] UploadImageDto UploadImageDto)
    {
        var fileData = new FormFileAdapter(file);
        bool isImage = imageService.VerifyImageAsync(fileData);

        if (!isImage)
        {
            return BadRequest("File is not image.");
        }

        var user = await userRepository.GetUserByIdAsync(UploadImageDto.UserId);

        if(user is null)
        {
            return BadRequest("User not found");
        }

        var imageHash = await imageService.UploadFileAsync(fileData);

        if (string.IsNullOrEmpty(imageHash))
        {
            return BadRequest("Failed to upload image.");
        }

        string imageFormat = imageService.GetEncodingFormat();
        string imageUrl = imageService.GetImageLinkById(imageHash);

        var tags = await tagService.VerifyTags(UploadImageDto.Tags);

        await imageRepository.CreateImageAsync(imageHash, imageUrl, imageFormat, UploadImageDto.UserId, tags);

        return Ok(new { ImageId = imageHash, Url = imageUrl });
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<ActionResult> DeleteImage(long id)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claimUserIdentifier = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        long claimUserId = Convert.ToInt64(claimUserIdentifier.Value);


        var existImage = await imageRepository.GetImageByIdAsync(id);
        if (existImage is null || claimUserId != existImage.UserEntityId)
        {
            return BadRequest("Image is not found.");
        }

        var deleteStatus = await imageService.DeleteImageByIdAsync(existImage.FileHash);

        if (deleteStatus == System.Net.HttpStatusCode.BadRequest)
        {
            return BadRequest("Failed to delete the image. Please try again later.");
        }

        await imageRepository.DeleteImageAsync(id);

        return NoContent();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult> GetImage(long id)
    {
        var imageDb = await imageRepository.GetImageByIdAsync(id);
        if (imageDb is null)
        {
            return BadRequest("Image is not found.");
        }
        return Content(imageDb.FilePath, "image/webp");
    }


    //[HttpGet("GetImagesByTag/{tag}")]
    //public async Task<ActionResult> GetImagesByTag(string tag)
    //{
    //    tag = tag.ToLower();
    //    string[] tagsArray = [tag];

    //    var imagesByTag = await imageRepository.GetImagesByTagsAsync(tagsArray);
    //    if (!imagesByTag.Any())
    //        return NotFound("Images by tag is not found.");

    //    List<byte[]> images = new List<byte[]>();
    //    foreach ( var image in imagesByTag)
    //    {
    //        var imageBytes = await imageService.ImageToByteArray(image.FilePath);
    //        images.Add(imageBytes);
    //    }

    //    return File(images, "image/jpeg");
    //}

    //[HttpGet("GetImagesByTags/{tags}")]
    //public async Task<ActionResult> GetImagesByTags(string tags)
    //{
    //    tags = tags.ToLower();
    //    var tagsArray = tags.Split(" ");

    //    var imagesByTags = await imageRepository.GetImagesByTagsAsync(tagsArray);
    //    if (!imagesByTags.Any())
    //        return NotFound("Images by tags is not found.");

    //    return Ok(imagesByTags);
    //}
}
