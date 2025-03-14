using ImageBox.Shared.Interfaces;
using System.Net;

namespace ImageBox.BusinessLogic.Interfaces
{
    public interface IImageS3Service
    {
        string GetEncodingFormat();
        Task<string?> UploadFileAsync(IFileData file);
        string GetImageLinkById(string id);
        Task<HttpStatusCode> DeleteImageByIdAsync(string id);
        bool VerifyImageAsync(IFileData fileData);
    }
}