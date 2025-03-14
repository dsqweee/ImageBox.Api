using ImageBox.DataAccess.Entities;

namespace ImageBox.BusinessLogic.Interfaces
{
    public interface ITagService
    {
        Task<List<TagEntity>> VerifyTags(string[] tags);
    }
}