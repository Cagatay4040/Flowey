using Flowey.BUSINESS.Features.Steps.Commands;
using Flowey.BUSINESS.Features.Steps.Queries;
using Flowey.CORE.DTO.Step;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class StepController : ControllerBase
    {
        private readonly ISender _sender;

        public StepController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("GetBoardData")]
        public async Task<IActionResult> GetBoardData(
            [FromQuery] Guid projectId, 
            [FromQuery] List<Guid> userIds, 
            [FromQuery] bool includeUnassigned = false, 
            [FromQuery] List<PriorityType>? priorities = null)
        {
            var result = await _sender.Send(new GetBoardDataQuery(projectId, userIds, includeUnassigned, priorities));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetProjectSteps")]
        public async Task<IActionResult> GetProjectSteps([FromQuery] Guid projectId)
        {
            var result = await _sender.Send(new GetProjectStepsQuery(projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddStep")]
        public async Task<IActionResult> AddStep([FromBody] StepAddDTO step)
        {
            var result = await _sender.Send(new AddStepCommand(step.Name, step.Order,step.Category, step.ProjectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddSteps")]
        public async Task<IActionResult> AddSteps([FromBody] List<StepAddDTO> steps)
        {
            var result = await _sender.Send(new AddRangeStepCommand(steps));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateStep")]
        public async Task<IActionResult> UpdateStep([FromBody] StepUpdateDTO step)
        {
            var result = await _sender.Send(new UpdateStepCommand(step.StepId, step.Name, step.Order, step.Category));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{projectId}/UpdateSteps")]
        public async Task<IActionResult> UpdateSteps(Guid projectId, [FromBody] List<StepUpdateDTO> steps)
        {
            var result = await _sender.Send(new UpdateRangeStepCommand(steps, projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteStep")]
        public async Task<IActionResult> DeleteStep([FromBody] StepDeleteDTO step)
        {
            var result = await _sender.Send(new SoftDeleteStepCommand(step.StepId, step.TargetStepId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
