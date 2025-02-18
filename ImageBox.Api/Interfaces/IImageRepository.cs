using ImageBox.Api.DataBase.Entity;

namespace ImageBox.Api.Interfaces;

public interface IImageRepository : IRepository<ImageEntity>
{
    //Task CreateImageAsync(ImageEntity image);
    //Task DeleteImageAsync(long imageId);
    Task<ImageEntity> GetImageByHash(string imageHash);
    //Task<ImageEntity> GetImageByIdAsync(long imageId);
    //Task UpdateImageAsync(ImageEntity image);
}