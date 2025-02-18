using ImageBox.Api.DataBase;
using ImageBox.Api.DataBase.Entity;
using ImageBox.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.Api.Repositories;

public class ImageRepository : Repository<ImageEntity>, IImageRepository
{
    public ImageRepository(ImageBoxDbContext dbContext) : base(dbContext)
    {
        
    }

    //public async Task CreateImageAsync(ImageEntity image)
    //{
    //    _dbContext.images.Add(image);
    //    await _dbContext.SaveChangesAsync();
    //}

    //public async Task DeleteImageAsync(long imageId)
    //{
    //    var imageEntity = new ImageEntity { Id = imageId };

    //    _dbContext.images.Remove(imageEntity);
    //    await _dbContext.SaveChangesAsync();
    //}

    //public async Task UpdateImageAsync(ImageEntity image)
    //{
    //    var existImage = await _dbContext.images.FindAsync(image.Id);
    //    _dbContext.Entry<ImageEntity>(existImage).CurrentValues.SetValues(image);
    //    await _dbContext.SaveChangesAsync();
    //}

    //public async Task<ImageEntity> GetImageByIdAsync(long imageId)
    //{
    //    return await _dbContext.images.FindAsync(imageId);
    //}

    public async Task<ImageEntity> GetImageByHash(string imageHash)
    {
        return await _dbContext.images.FirstOrDefaultAsync(x => x.FileHash == imageHash);
    }
}
