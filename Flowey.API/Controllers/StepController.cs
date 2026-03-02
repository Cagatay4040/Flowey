using Flowey.API.Attributes;
using Flowey.BUSINESS.DTO.Step;
using Flowey.BUSINESS.Features.Steps.Commands;
using Flowey.BUSINESS.Features.Steps.Queries;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [StepAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetBoardData([FromQuery] Guid projectId, [FromQuery] List<Guid> userIds, [FromQuery] bool includeUnassigned = false)
        {
            var result = await _sender.Send(new GetBoardDataQuery(projectId, userIds, includeUnassigned));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetProjectSteps")]
        [StepAuthorize(RoleType.Admin, RoleType.Editor, RoleType.Member)]
        public async Task<IActionResult> GetProjectSteps([FromQuery] Guid projectId)
        {
            var result = await _sender.Send(new GetProjectStepsQuery(projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddStep")]
        [StepAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> AddStep([FromBody] StepAddDTO step)
        {
            var result = await _sender.Send(new AddStepCommand(step));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddSteps")]
        [StepAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> AddSteps([FromBody] List<StepAddDTO> steps)
        {
            var result = await _sender.Send(new AddRangeStepCommand(steps));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateStep")]
        [StepAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> UpdateStep([FromBody] StepUpdateDTO step)
        {
            var result = await _sender.Send(new UpdateStepCommand(step));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdateSteps")]
        [StepAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> UpdateSteps([FromBody] List<StepUpdateDTO> steps)
        {
            var result = await _sender.Send(new UpdateRangeStepCommand(steps));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeleteStep")]
        [StepAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> DeleteStep([FromBody] StepDeleteDTO stepDto)
        {
            var result = await _sender.Send(new SoftDeleteStepCommand(stepDto));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
