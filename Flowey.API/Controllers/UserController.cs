using Microsoft.AspNetCore.Authorization;
using System.Text;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserAddDTO dto)
        {
            var result = await _userService.AddAsync(dto);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeDTO dto)
        {
            var result = await _userService.ChangePasswordAsync(dto);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
        {
            var result = await _userService.UpdateAsync(dto);

            return Ok(result);
        }
    }
}
