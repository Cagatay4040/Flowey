using Flowey.BUSINESS.Features.Notifications.Commands;
using Flowey.BUSINESS.Features.Notifications.Queries;
using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserNotificationController : ControllerBase
    {
        private readonly ISender _sender;

        public UserNotificationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("GetNotification")]
        public async Task<IActionResult> GetNotification([FromQuery] Guid userId)
        {
            var result = await _sender.Send(new GetUserNotificationsQuery(userId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddNotification")]
        public async Task<IActionResult> AddNotification([FromBody] UserNotificationAddDTO request)
        {
            var result = await _sender.Send(new CreateUserNotificationCommand(request.UserId, request.SenderId, request.Title, request.Message, request.ActionUrl, request.IsRead));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromQuery] Guid notificationId)
        {
            var result = await _sender.Send(new MarkAsReadCommand(notificationId)); ;
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] Guid userId)
        {
            var result = await _sender.Send(new MarkAllAsReadCommand(userId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
