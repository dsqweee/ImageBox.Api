using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.DataAccess.Repositories;

public class ImageRepository : Repository<ImageEntity>, IImageRepository
{
    public ImageRepository(ImageBoxDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<ImageEntity> GetImageByHash(string imageHash)
    {
        return await _dbContext.images.FirstOrDefaultAsync(x => x.FileHash == imageHash);
    }
}
