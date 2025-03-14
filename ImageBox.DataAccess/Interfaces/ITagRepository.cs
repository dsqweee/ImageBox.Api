using ImageBox.DataAccess.Entities;

namespace ImageBox.DataAccess.Interfaces;

public interface ITagRepository //: IRepository<TagEntity>
{
    Task CreateTagAsync(TagEntity entity);
    Task DeleteTagAsync(long Id);
    Task<TagEntity?> GetTagByNameAsync(string tag);
    Task UpdateTagAsync(TagEntity entity);

    Task<List<TagEntity>> GetDataBaseStateForAnyTags(string[] entity);
}
