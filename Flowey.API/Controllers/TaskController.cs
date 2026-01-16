using Flowey.API.Attributes;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService tasktService)
        {
            _taskService = tasktService;
        }

        [HttpGet("GetProjectTasks")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetProjectTasks([FromBody] Guid projectId)
        {
            var result = await _taskService.GetProjectTasksAsync(projectId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetTaskHistory")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetTaskHistory([FromQuery] Guid taskId)
        {
            var result = await _taskService.GetTaskHistoryAsync(taskId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> AddTask([FromBody] TaskAddDTO task)
        {
            var result = await _taskService.AddAndAssignTaskAsync(task);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ChangeAssignTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> ChangeAssignTask([FromBody] TaskAssignDTO task)
        {
            var result = await _taskService.ChangeAssignTaskAsync(task);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ChangeStepTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> ChangeStepTask([FromBody] TaskStepDTO task)
        {
            var result = await _taskService.ChangeStepTaskAsync(task);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateDTO task)
        {
            var result = await _taskService.UpdateAsync(task);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> DeleteTask([FromBody] Guid taskId)
        {
            var result = await _taskService.SoftDeleteAsync(taskId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
