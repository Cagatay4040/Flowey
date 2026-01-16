using Flowey.API.Attributes;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        [HttpGet("UserProjects")]
        public async Task<IActionResult> UserProjects()
        {
            var result = await _projectService.GetProjectsByLoginUserAsync();
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("MyProjects")]
        public async Task<IActionResult> MyProjects()
        {
            var result = await _projectService.GetMyProjectsAsync();
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("ProjectUsers")]
        public async Task<IActionResult> ProjectUsers([FromQuery] Guid projectId)
        {
            var result = await _projectService.GetProjectUsersAsync(projectId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] ProjectAddDTO project)
        {
            var result = await _projectService.AddWithCreatorAsync(project);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddUserToProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> AddUserToProject([FromBody] ProjectUserAddDTO projectUser)
        {
            var result = await _projectService.AddUserToProjectAsync(projectUser);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("Update")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDTO project)
        {
            var result = await _projectService.UpdateAsync(project);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("Delete")]
        [ProjectAuthorize(RoleType.Admin)]
        public async Task<IActionResult> DeleteProject([FromBody] Guid projectId)
        {
            var result = await _projectService.SoftDeleteAsync(projectId);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("RemoveUserFromProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] ProjectRemoveUserDTO projectUser)
        {
            var result = await _projectService.RemoveUserFromProjectAsync(projectUser);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
