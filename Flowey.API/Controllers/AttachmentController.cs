using Flowey.CORE.DTO.Attachment;
using Flowey.CORE.Interfaces.Services;
using Flowey.SHARED.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly ILocalFileStorageService _fileService;

        public AttachmentController(ILocalFileStorageService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDTO uploadDto)
        {
            var result = await _fileService.UploadAsync(uploadDto.File);
            if (result.ResultStatus == ResultStatus.Success) return Ok(new { url = result.Data });
            return BadRequest(result.Message);
        }
    }
}
