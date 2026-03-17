using Flowey.BUSINESS.Features.Subscription.Commands;
using Flowey.BUSINESS.Features.Subscription.Queries;
using Flowey.CORE.DTO.User;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISender _sender;

        public SubscriptionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("BillingHistory")]
        public async Task<IActionResult> BillingHistory([FromQuery] Guid userId)
        {
            var result = await _sender.Send(new GetBillingHistoryQuery(userId));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] UserCheckoutRequestDTO request)
        {
            var result = await _sender.Send(new CheckoutCommand(request.MonthsToPurchase));
            if (result.ResultStatus == ResultStatus.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
