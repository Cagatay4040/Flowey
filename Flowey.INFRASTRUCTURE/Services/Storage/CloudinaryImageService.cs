using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Flowey.INFRASTRUCTURE.Services.Storage
{
    public class CloudinaryImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),

                    Transformation = new Transformation()
                        .Height(500).Width(500)
                        .Crop("fill")
                        .Gravity("face"),

                    Folder = "flowey_profiles"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult.SecureUrl?.ToString();
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }
    }
}