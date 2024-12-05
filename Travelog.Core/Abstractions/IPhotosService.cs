using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
namespace Travelog.Core.Abstractions
{
    public interface IPhotosService
    {

        Task<string> SavePhotoAsync(IFormFile file);
        Task<string> SavePhotoAsyncLocal(IFormFile file,string baseUrl);
        Result<string> GetPhotoAsBase64(string filePath);
        Result<FileStream> GetPhotoAsFormFile(string filePath);
    }
}
