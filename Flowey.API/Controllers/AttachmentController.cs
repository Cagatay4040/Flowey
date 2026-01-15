using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Attachment;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IFileService _fileService;

        public AttachmentController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDTO uploadDto)
        {
            var result = await _fileService.UploadAsync(uploadDto.File);

            if (result.ResultStatus == ResultStatus.Success)
            {
                return Ok(new { url = result.Data });
            }

            return BadRequest(result.Message);
        }
    }
}
