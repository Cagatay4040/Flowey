using Microsoft.AspNetCore.Http;

namespace Flowey.CORE.Interfaces.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
