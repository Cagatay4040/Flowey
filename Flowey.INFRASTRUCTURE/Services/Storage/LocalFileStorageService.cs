using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Flowey.INFRASTRUCTURE.Services.Storage
{
    public class LocalFileStorageService : ILocalFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IDataResult<string>> UploadAsync(IFormFile file)
        {
            string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var fileUrl = $"{baseUrl}/uploads/{uniqueFileName}";

            return new DataResult<string>(ResultStatus.Success, fileUrl, Messages.FileUploadSuccessfull);
        }
    }
}
