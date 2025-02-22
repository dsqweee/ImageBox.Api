using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class ImageRepository : Repository<ImageEntity>, IImageRepository
{
    public ImageRepository(ImageBoxDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<ImageEntity?> GetImageByHashAsync(string imageHash)
    {
        return await _dbContext.images.FirstOrDefaultAsync(x => x.FileHash == imageHash);
    }

    public async Task<List<ImageEntity>> GetImagesByTagsAsync(string[] tags)
    {
        return await _dbContext.images.Include(x => x.Tags)
                                      .Where(image => image.Tags.Any(tag => tags.Contains(tag.Tag)))
                                      .ToListAsync();
    }

    public async Task<ImageEntity?> GetImageIncludeTagsByIdAsync(long imageId)
    {
        return await _dbContext.images.Include(x => x.Tags)
                                      .FirstOrDefaultAsync(x=>x.Id == imageId);
    }
}
