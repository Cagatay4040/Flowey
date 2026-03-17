using Flowey.CORE.DTO.User;
using Flowey.BUSINESS.Features.Users.Commands;
using Flowey.BUSINESS.Features.Users.Queries;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new List<UserSelectDTO>());

            var users = await _sender.Send(new SearchUsersQuery(searchTerm));

            return Ok(users);
        }

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
        {
            var result = await _sender.Send(new UpdateUserCommand(dto.Id, dto.Email, dto.Name, dto.Surname));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("ProfileImage")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var result = await _sender.Send(new UpdateProfileImageCommand(file));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
