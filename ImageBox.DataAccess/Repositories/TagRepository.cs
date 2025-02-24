using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class TagRepository : Repository<TagEntity>, ITagRepository
{
    public TagRepository(ImageBoxDbContext dbContext) : base(dbContext)
    {

    }


    public async Task<TagEntity?> GetTagByName(string tag)
    {
        return await _dbContext.tags.FirstOrDefaultAsync(x => x.Tag == tag);
    }


    public async Task CreateRangeUniqueAsync(TagEntity[] entity)
    {
        List<TagEntity> uniqueEntity = new List<TagEntity>();

        foreach (TagEntity entityItem in entity)
        {
            var isAny = await _dbContext.tags.AnyAsync(x => x.Tag == entityItem.Tag);
            if(!isAny)
                uniqueEntity.Add(entityItem);
        }

        if(uniqueEntity.Any())
        {
            await _dbContext.tags.AddRangeAsync(uniqueEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
