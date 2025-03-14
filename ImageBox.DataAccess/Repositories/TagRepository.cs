using EFCore.BulkExtensions;
using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq;

namespace ImageBox.DataAccess.Repositories;

public class TagRepository : /*Repository<TagEntity>(dbContext),*/ ITagRepository
{
    private readonly ImageBoxDbContext _dbContext;
    public TagRepository(ImageBoxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateTagAsync(TagEntity entity)
    {
        await _dbContext.tags.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(long Id)
    {
        var entity = await _dbContext.tags.FindAsync(Id);

        _dbContext.tags.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTagAsync(TagEntity entity)
    {
        var existEntity = await _dbContext.tags.FindAsync(entity);

        _dbContext.Entry<TagEntity>(existEntity).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TagEntity?> GetTagByNameAsync(string tag)
    {
        return await _dbContext.tags.FirstOrDefaultAsync(x => x.Tag == tag);
    }

    public async Task<List<TagEntity>> GetDataBaseStateForAnyTags(string[] tags)
    {
        var existingInDbTags = await _dbContext.tags.Where(t => tags.Contains(t.Tag))
                                                .ToListAsync();

        var tagsList = tags.Select(x => new TagEntity { Tag = x });

        var tagsWithoutDuplicate = existingInDbTags.UnionBy(tagsList, x=>x.Tag).ToList();

        return tagsWithoutDuplicate; 



        //List<TagEntity> uniqueEntity = new List<TagEntity>();

        //foreach (string entityItem in tags)
        //{
        //    var isAny = await _dbContext.tags.AnyAsync(x => x.Tag == entityItem);
        //    if (!isAny)
        //        uniqueEntity.Add(new TagEntity { Tag = entityItem });
        //}

        //if (uniqueEntity.Any())
        //{
        //    await _dbContext.tags.AddRangeAsync(uniqueEntity);
        //    await _dbContext.SaveChangesAsync();
        //}
    }
}
