using Flowey.BUSINESS.Features.Comments.Commands;
using Flowey.BUSINESS.Features.Comments.Queries;
using Flowey.CORE.DTO.Comment;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetByTaskId(Guid taskId)
        {
            var result = await _sender.Send(new GetCommentsByTaskIdQuery(taskId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CommentAddDTO dto)
        {
            var result = await _sender.Send(new AddCommentCommand(dto.Content, dto.TaskId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateComment")]
        public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDTO dto)
        {
            var result = await _sender.Send(new UpdateCommentCommand(dto.CommentId, dto.Content));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteComment/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var result = await _sender.Send(new DeleteCommentCommand(commentId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
