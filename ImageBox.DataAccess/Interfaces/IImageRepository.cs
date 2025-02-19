using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface IImageRepository : IRepository<ImageEntity>
{
    Task<ImageEntity> GetImageByHash(string imageHash);
}