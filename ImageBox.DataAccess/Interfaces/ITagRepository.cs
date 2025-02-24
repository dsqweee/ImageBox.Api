using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface ITagRepository : IRepository<TagEntity>
{
    Task<TagEntity?> GetTagByName(string tag);
    Task CreateRangeUniqueAsync(TagEntity[] entity);
}
