using Flowey.API.Attributes;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.BUSINESS.Features.Projects.Commands;
using Flowey.BUSINESS.Features.Projects.Queries;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ISender _sender;

        public ProjectController(ISender sender)
        {
            _sender = sender;
        }
        
        [HttpGet("UserProjects")]
        public async Task<IActionResult> UserProjects()
        {
            var result = await _sender.Send(new GetUserProjectsQuery());
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("MyProjects")]
        public async Task<IActionResult> MyProjects()
        {
            var result = await _sender.Send(new GetMyProjectsQuery());
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("ProjectUsers")]
        public async Task<IActionResult> ProjectUsers([FromQuery] Guid projectId)
        {
            var result = await _sender.Send(new GetProjectUsersQuery(projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddProject")]
        [Authorize(Policy = "RequirePremium")]
        public async Task<IActionResult> AddProject([FromBody] ProjectAddDTO project)
        {
            var result = await _sender.Send(new AddProjectWithCreatorCommand(project));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddUserToProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> AddUserToProject([FromBody] ProjectUserAddDTO projectUser)
        {
            var result = await _sender.Send(new AddUserToProjectCommand(projectUser));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("Update")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDTO project)
        {
            var result = await _sender.Send(new UpdateProjectCommand(project));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("Delete")]
        [ProjectAuthorize(RoleType.Admin)]
        public async Task<IActionResult> DeleteProject([FromBody] Guid projectId)
        {
            var result = await _sender.Send(new SoftDeleteProjectCommand(projectId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("RemoveUserFromProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] ProjectRemoveUserDTO projectUser)
        {
            var result = await _sender.Send(new RemoveUserFromProjectCommand(projectUser));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
