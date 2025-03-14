using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface IImageRepository //: IRepository<ImageEntity>
{
    Task CreateImageAsync(string Hash, string Url, string Format, long UserId, List<TagEntity> Tags);
    Task DeleteImageAsync(long Id);
    Task<ImageEntity?> GetImageByIdAsync(long Id);
    Task UpdateImageAsync(ImageEntity entity);

    Task<ImageEntity?> GetImageByHashAsync(string imageHash);
    Task<ImageEntity?> GetImageIncludeTagsByIdAsync(long imageId);
    Task<List<ImageEntity?>> GetImagesByTagsAsync(string[] tags);
}