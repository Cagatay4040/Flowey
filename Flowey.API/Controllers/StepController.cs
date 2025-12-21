using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Step;
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

        [HttpGet("GetProjectSteps")]
        public async Task<IActionResult> GetProjectSteps([FromQuery] Guid projectId)
        {
            var result = await _stepService.GetProjectSteps(projectId);

            return Ok(result);
        }

        [HttpPost("AddStep")]
        public async Task<IActionResult> AddStep([FromBody] StepAddDTO step)
        {
            var result = await _stepService.AddStepAsync(step);

            return Ok(result);
        }

        [HttpPost("AddSteps")]
        public async Task<IActionResult> AddSteps([FromBody] List<StepAddDTO> steps)
        {
            var result = await _stepService.AddRangeStepAsync(steps);

            return Ok(result);
        }

        [HttpPut("UpdateStep")]
        public async Task<IActionResult> UpdateStep([FromBody] StepUpdateDTO step)
        {
            var result = await _stepService.UpdateStepAsync(step);

            return Ok(result);
        }

        [HttpPut("UpdateSteps")]
        public async Task<IActionResult> UpdateSteps([FromBody] List<StepUpdateDTO> steps)
        {
            var result = await _stepService.UpdateRangeStepAsync(steps);

            return Ok(result);
        }

        [HttpDelete("DeleteStep")]
        public async Task<IActionResult> DeleteStep([FromBody] Guid stepId)
        {
            var result = await _stepService.SoftDeleteAsync(stepId);

            return Ok(result);
        }
    }
}
