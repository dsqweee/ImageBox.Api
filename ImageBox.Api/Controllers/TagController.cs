using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImageBox.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class TagController(ITagRepository tagRepository) : ControllerBase
{
    [HttpPost("{tag}")]
    public async Task<ActionResult> CreateTag(string tag)
    {
        tag = tag.ToLower();
        var existTag = await tagRepository.GetTagByNameAsync(tag);
        if (existTag is not null)
        {
            return BadRequest("Tag is already added.");
        }

        var tagEntity = new TagEntity { Tag = tag };
        await tagRepository.CreateTagAsync(tagEntity);

        return Ok();
    }

    [HttpGet("{tag}")]
    public async Task<ActionResult> GetTag(string tag)
    {
        tag = tag.ToLower();
        var existTag = await tagRepository.GetTagByNameAsync(tag);
        if (existTag is null)
        {
            return BadRequest("Tag was not found.");
        }

        return Ok(existTag);
    }
}
