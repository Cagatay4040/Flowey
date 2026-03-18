using Flowey.CORE.Result.Abstract;
using Microsoft.AspNetCore.Http;

namespace Flowey.CORE.Interfaces.Services
{
    public interface ILocalFileStorageService
    {
        Task<IDataResult<string>> UploadAsync(IFormFile file);
    }
}