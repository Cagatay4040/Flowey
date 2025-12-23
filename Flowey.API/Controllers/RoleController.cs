using Flowey.BUSINESS.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetUserRole")]
        public async Task<IActionResult> GetUserRole([FromBody] Guid projectId)
        {
            var result = await _roleService.GetUserRole(projectId);

            return Ok(result);
        }
    }
}
