using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface IImageRepository : IRepository<ImageEntity>
{
    Task<ImageEntity?> GetImageByHashAsync(string imageHash);
    Task<ImageEntity?> GetImageIncludeTagsByIdAsync(long imageId);
    Task<List<ImageEntity?>> GetImagesByTagsAsync(string[] tags);
}