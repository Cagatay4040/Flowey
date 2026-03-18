using Microsoft.AspNetCore.Http;

namespace Flowey.CORE.DTO.Attachment
{
    public class FileUploadDTO
    {
        public IFormFile File { get; set; }
    }
}
