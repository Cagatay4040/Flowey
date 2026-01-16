using Flowey.API.Attributes;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Step;
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
    public class StepController : ControllerBase
    {
        private readonly IStepService _stepService;

        public StepController(IStepService stepService)
        {
            _stepService = stepService;
        }

        [HttpGet("GetBoardData")]
        public async Task<IActionResult> GetProjectSteps([FromQuery] Guid projectId, [FromQuery] List<Guid> userIds, [FromQuery] bool includeUnassigned = false)
        {
            var result = await _stepService.GetBoardDataAsync(projectId, userIds, includeUnassigned);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddStep")]
        [StepAuthorize(RoleType.Admin)]
        public async Task<IActionResult> AddStep([FromBody] StepAddDTO step)
        {
            var result = await _stepService.AddStepAsync(step);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddSteps")]
        [StepAuthorize(RoleType.Admin)]
        public async Task<IActionResult> AddSteps([FromBody] List<StepAddDTO> steps)
        {
            var result = await _stepService.AddRangeStepAsync(steps);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateStep")]
        [StepAuthorize(RoleType.Admin)]
        public async Task<IActionResult> UpdateStep([FromBody] StepUpdateDTO step)
        {
            var result = await _stepService.UpdateStepAsync(step);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateSteps")]
        [StepAuthorize(RoleType.Admin)]
        public async Task<IActionResult> UpdateSteps([FromBody] List<StepUpdateDTO> steps)
        {
            var result = await _stepService.UpdateRangeStepAsync(steps);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteStep")]
        [StepAuthorize(RoleType.Admin)]
        public async Task<IActionResult> DeleteStep([FromBody] Guid stepId)
        {
            var result = await _stepService.SoftDeleteAsync(stepId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
