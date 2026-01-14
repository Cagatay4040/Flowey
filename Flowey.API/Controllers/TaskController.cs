using Flowey.API.Attributes;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
using Flowey.CORE.Enums;
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

            return Ok(result);
        }

        [HttpGet("GetTaskHistory")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetTaskHistory([FromQuery] Guid taskId)
        {
            var result = await _taskService.GetTaskHistoryAsync(taskId);

            return Ok(result);
        }

        [HttpPost("AddTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> AddTask([FromBody] TaskAddDTO task)
        {
            var result = await _taskService.AddAndAssignTaskAsync(task);

            return Ok(result);
        }

        [HttpPost("ChangeAssignTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> ChangeAssignTask([FromBody] TaskAssignDTO task)
        {
            var result = await _taskService.ChangeAssignTaskAsync(task);

            return Ok(result);
        }

        [HttpPost("ChangeStepTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> ChangeStepTask([FromBody] TaskStepDTO task)
        {
            var result = await _taskService.ChangeStepTaskAsync(task);

            return Ok(result);
        }

        [HttpPut("UpdateTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateDTO task)
        {
            var result = await _taskService.UpdateAsync(task);

            return Ok(result);
        }

        [HttpDelete("DeleteTask")]
        [TaskAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> DeleteTask([FromBody] Guid taskId)
        {
            var result = await _taskService.SoftDeleteAsync(taskId);

            return Ok(result);
        }
    }
}
