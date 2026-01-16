using Flowey.API.Attributes;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Comment;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("task/{taskId}")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetByTaskId(Guid taskId)
        {
            var result = await _commentService.GetByTaskIdAsync(taskId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddComment")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> AddComment([FromBody] CommentAddDTO dto)
        {
            var result = await _commentService.AddAsync(dto);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateComment")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDTO dto)
        {
            var result = await _commentService.UpdateAsync(dto);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteComment/{commentId}")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var result = await _commentService.DeleteAsync(commentId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
