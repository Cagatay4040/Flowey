using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Flowey.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new List<UserSelectDTO>());

            var users = await _userService.SearchUsersAsync(searchTerm);

            return Ok(users);
        }

        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
        {
            var result = await _userService.UpdateAsync(dto);
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
