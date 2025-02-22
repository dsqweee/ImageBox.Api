using ImageBox.BusinessLogic.Interfaces;
using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using ImageBox.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageBox.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ImageController(IImageService imageService, IImageRepository imageRepository, ITagRepository tagRepository) : ControllerBase
{

    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)]
    [Authorize]
    public async Task<ActionResult> UploadImage(IFormFile file, long userId, [FromQuery] string[] tags)
    {
        var fileData = new FormFileAdapter(file);
        bool isImage = imageService.VerifyImageAsync(fileData);

        if (!isImage)
            return BadRequest("File is not image.");

        (string? imagePath, string? imageHash) imageInfo = await imageService.UploadImageAsync(fileData);

        if (imageInfo.imagePath is null || imageInfo.imageHash is null)
            return BadRequest("Failed to save the image. Please try again later.");

        string imageFormat = imageService.GetEncodingFormat();

        List<TagEntity> tagEntities = new List<TagEntity>();

        foreach (var tag in tags)
        {
            var tagDb = await tagRepository.GetTagByName(tag);
            if (tagDb is null)
                tagDb = new TagEntity { Tag = tag };

            tagEntities.Add(tagDb);
        }

        var imageEntity = new ImageEntity
        {
            FileHash = imageInfo.imageHash,
            FilePath = imageInfo.imagePath,
            FileSize = 0,
            FileType = imageFormat,
            UserEntityId = userId,
            Tags = tagEntities
        };

        await imageRepository.CreateAsync(imageEntity);

        return Ok();
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<ActionResult> DeleteImage(long id)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claimUserIdentifier = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        long claimUserId = Convert.ToInt64(claimUserIdentifier.Value);


        var existImage = await imageRepository.GetByIdAsync(id);
        if (existImage is null || claimUserId != existImage.UserEntityId)
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
