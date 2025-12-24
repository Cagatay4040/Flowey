using Flowey.API.Attributes;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.CORE.Enums;
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

            return Ok(result);
        }

        [HttpGet("MyProjects")]
        public async Task<IActionResult> MyProjects()
        {
            var result = await _projectService.GetMyProjectsAsync();

            return Ok(result);
        }

        [HttpGet("ProjectUsers")]
        public async Task<IActionResult> ProjectUsers([FromQuery] Guid projectId)
        {
            var result = await _projectService.GetProjectUsersAsync(projectId);

            return Ok(result);
        }

        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] ProjectAddDTO project)
        {
            var result = await _projectService.AddWithCreatorAsync(project);

            return Ok(result);
        }

        [HttpPost("AddUserToProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> AddUserToProject([FromBody] ProjectUserAddDTO projectUser)
        {
            var result = await _projectService.AddUserToProjectAsync(projectUser);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDTO project)
        {
            var result = await _projectService.UpdateAsync(project);

            return Ok(result);
        }

        [HttpDelete("Delete")]
        [ProjectAuthorize(RoleType.Admin)]
        public async Task<IActionResult> DeleteProject([FromBody] Guid projectId)
        {
            var result = await _projectService.SoftDeleteAsync(projectId);

            return Ok(result);
        }

        [HttpDelete("RemoveUserFromProject")]
        [ProjectAuthorize(RoleType.Admin, RoleType.Editor)]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] ProjectRemoveUserDTO projectUser)
        {
            var result = await _projectService.RemoveUserFromProjectAsync(projectUser);

            return Ok(result);
        }
    }
}
