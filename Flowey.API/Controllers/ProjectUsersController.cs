using Flowey.API.Attributes;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.BUSINESS.Features.ProjectUsers.Commands;
using Flowey.BUSINESS.Features.ProjectUsers.Queries;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProjectUsersController : ControllerBase
    {
        private readonly ISender _sender;

        public ProjectUsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("ProjectUsers")]
        public async Task<IActionResult> ProjectUsers([FromQuery] Guid projectId)
        {
            var result = await _sender.Send(new GetProjectUsersQuery(projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddUserToProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> AddUserToProject([FromBody] ProjectUserAddDTO projectUser)
        {
            var result = await _sender.Send(new AddUserToProjectCommand(projectUser.UserId, projectUser.ProjectId, projectUser.RoleId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("Transfer-ownership")]
        [ProjectAuthorize(RoleType.Admin)]
        public async Task<IActionResult> TransferOwnership([FromBody] TransferOwnershipDTO dto)
        {
            var result = await _sender.Send(new TransferOwnershipCommand(dto.ProjectId, dto.NewOwnerId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("RemoveUserFromProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] ProjectRemoveUserDTO projectUser)
        {
            var result = await _sender.Send(new RemoveUserFromProjectCommand(projectUser.UserId, projectUser.ProjectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
