using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserNotificationController : ControllerBase
    {
        private readonly IUserNotificationService _userNotificationService;

        public UserNotificationController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        [HttpGet("GetNotification")]
        public async Task<IActionResult> GetNotification([FromQuery] Guid userId)
        {
            var result = await _userNotificationService.GetUserNotificationsAsync(userId);

            if (result.ResultStatus == ResultStatus.Success) return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("AddNotification")]
        public async Task<IActionResult> AddNotification([FromBody] UserNotificationAddDTO request)
        {
            var result = await _userNotificationService.AddUserNotificationAsync(request);

            if (result.ResultStatus == ResultStatus.Success) return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromQuery] Guid notificationId)
        {
            var result = await _userNotificationService.MarkAsReadAsync(notificationId);

            if (result.ResultStatus == ResultStatus.Success) return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] Guid userId)
        {
            var result = await _userNotificationService.MarkAllAsReadAsync(userId);

            if (result.ResultStatus == ResultStatus.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}
