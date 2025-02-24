using ImageBox.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ImageBox.BusinessLogic.Interfaces
{
    public interface IImageS3Service
    {
        string GetEncodingFormat();
        Task<string> UploadFileAsync(IFileData file);
        Task<string> GetImageLinkByIdAsync(string id);
        Task<HttpStatusCode> DeleteImageByIdAsync(string id);
        bool VerifyImageAsync(IFileData fileData);
    }
}