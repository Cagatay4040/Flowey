using Flowey.API.Attributes;
using Flowey.BUSINESS.DTO.Comment;
using Flowey.BUSINESS.Features.Comments.Queries;
using Flowey.BUSINESS.Features.Comments.Commands;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using MediatR;
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
        private readonly ISender _sender;

        public CommentController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("task/{taskId}")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetByTaskId(Guid taskId)
        {
            var result = await _sender.Send(new GetCommentsByTaskIdQuery(taskId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddComment")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> AddComment([FromBody] CommentAddDTO dto)
        {
            var result = await _sender.Send(new AddCommentCommand(dto.Content, dto.TaskId, dto.UserId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateComment")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDTO dto)
        {
            var result = await _sender.Send(new UpdateCommentCommand(dto.CommentId, dto.Content));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteComment/{commentId}")]
        [CommentAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var result = await _sender.Send(new DeleteCommentCommand(commentId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
