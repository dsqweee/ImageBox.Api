using ImageBox.BusinessLogic.Interfaces;
using ImageBox.DataAccess.Entities;
using ImageBox.DataAccess.Interfaces;
using ImageBox.DataAccess.Repositories;

namespace ImageBox.BusinessLogic.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagEntity>> VerifyTags(string[] tags)
        {
            var dataCheckedList = AllowGoodTags(tags);

            if (!dataCheckedList.Any())
                return new List<TagEntity>();

            var tagsList = await _tagRepository.GetDataBaseStateForAnyTags(dataCheckedList);
            return tagsList;
        }

        private string[] BannedSymbols = ["⠀"];

        private static string[] AllowGoodTags(string[] tags)
        {
            List<string> tagList = new();

            foreach (var tag in tags)
            {
                if(!string.IsNullOrWhiteSpace(tag))
                {
                    string tagLower = tag.ToLower();
                    tagList.Add(tagLower);
                }
            }

            return tagList.ToArray();
        }

    }
}
