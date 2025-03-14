using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class ImageRepository : /*Repository<ImageEntity>(dbContext),*/ IImageRepository
{
    private readonly ImageBoxDbContext _dbContext;
    public ImageRepository(ImageBoxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateImageAsync(string Hash, string Url, string Format, long UserId, List<TagEntity> Tags)
    {
        var imageEntity = new ImageEntity
        {
            FileHash = Hash,
            FilePath = Url,
            FileType = Format,
            UserEntityId = UserId,
            Tags = Tags
        };

        await _dbContext.images.AddAsync(imageEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteImageAsync(long Id)
    {
        var entity = await _dbContext.images.FindAsync(Id);

        _dbContext.images.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateImageAsync(ImageEntity entity)
    {
        var existEntity = await _dbContext.images.FirstOrDefaultAsync(x => x.Id == entity.Id);

        _dbContext.Entry<ImageEntity>(existEntity).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ImageEntity?> GetImageByIdAsync(long Id)
    {
        return await _dbContext.images.FindAsync(Id);
    }

    public async Task<ImageEntity?> GetImageByHashAsync(string imageHash)
    {
        return await _dbContext.images.FirstOrDefaultAsync(x => x.FileHash == imageHash);
    }

    public async Task<List<ImageEntity>> GetImagesByTagsAsync(string[] tags)
    {
        return await _dbContext.tags.Include(x => x.Images)
                                    .Where(x => tags.Contains(x.Tag)).SelectMany(x => x.Images)
                                    .ToListAsync();
        //return await _dbContext.images.Include(x => x.Tags)
        //                              .Where(image => image.Tags.Any(tag => tags.Contains(tag.Tag)))
        //                              .ToListAsync();
    }

    public async Task<ImageEntity?> GetImageIncludeTagsByIdAsync(long imageId)
    {
        return await _dbContext.images.Include(x => x.Tags)
                                      .FirstOrDefaultAsync(x=>x.Id == imageId);
    }
}
