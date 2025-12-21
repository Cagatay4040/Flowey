using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
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
        public async Task<IActionResult> GetProjectTasks([FromBody] Guid projectId)
        {
            var result = await _taskService.GetProjectTasksAsync(projectId);

            return Ok(result);
        }

        [HttpPost("AddTask")]
        public async Task<IActionResult> AddTask([FromBody] TaskAddDTO task)
        {
            var result = await _taskService.AddAndAssignTaskAsync(task);

            return Ok(result);
        }

        [HttpPut("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateDTO task)
        {
            var result = await _taskService.UpdateAsync(task);

            return Ok(result);
        }

        [HttpDelete("DeleteTask")]
        public async Task<IActionResult> DeleteTask([FromBody] Guid taskId)
        {
            var result = await _taskService.SoftDeleteAsync(taskId);

            return Ok(result);
        }
    }
}
