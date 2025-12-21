using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Comment;
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
        public async Task<IActionResult> GetByTaskId(Guid taskId)
        {
            var result = await _commentService.GetByTaskIdAsync(taskId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CommentAddDTO dto)
        {
            var result = await _commentService.AddAsync(dto);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CommentUpdateDTO dto)
        {
            var result = await _commentService.UpdateAsync(dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _commentService.DeleteAsync(id);

            return Ok(result);
        }
    }
}
