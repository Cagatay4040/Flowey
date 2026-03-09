using Flowey.API.Attributes;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Features.Tasks.Commands;
using Flowey.BUSINESS.Features.Tasks.Queries;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ISender _sender;

        public TaskController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("GetProjectTasks")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetProjectTasks([FromQuery] Guid projectId)
        {
            var result = await _sender.Send(new GetProjectTasksQuery(projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetTaskHistory")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetTaskHistory([FromQuery] Guid taskId)
        {
            var result = await _sender.Send(new GetTaskHistoryQuery(taskId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> AddTask([FromBody] TaskAddDTO task)
        {
            var result = await _sender.Send(new AddTaskCommand(task.Title, task.Description, task.Priority, task.Deadline, task.ProjectId, task.UserId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ChangeAssignTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> ChangeAssignTask([FromBody] TaskAssignDTO task)
        {
            var result = await _sender.Send(new ChangeAssignTaskCommand(task.TaskId, task.UserId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ChangeStepTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> ChangeStepTask([FromBody] TaskStepDTO task)
        {
            var result = await _sender.Send(new ChangeStepTaskCommand(task.TaskId, task.NewStepId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateDTO task)
        {
            var result = await _sender.Send(new UpdateTaskCommand(task.TaskId, task.Title, task.Description, task.Priority, task.Deadline));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> DeleteTask([FromQuery] Guid taskId)
        {
            var result = await _sender.Send(new SoftDeleteTaskCommand(taskId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
