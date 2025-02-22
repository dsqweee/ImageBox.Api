using ImageBox.Shared.Interfaces;

namespace ImageBox.Api;

public class FormFileAdapter : IFileData
{
    private readonly IFormFile _file;

    public FormFileAdapter(IFormFile file)
    {
        _file = file;
    }

    public Stream OpenReadStream()
        => _file.OpenReadStream();
}
