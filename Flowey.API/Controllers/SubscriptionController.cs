using Flowey.CORE.DTO.User;
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
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("BillingHistory")]
        public async Task<IActionResult> BillingHistory([FromQuery] Guid userId)
        {
            var result = await _subscriptionService.GetBillingHistoryAsync(userId);

            if (result.ResultStatus == ResultStatus.Success) return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] UserCheckoutRequestDTO request)
        {
            var result = await _subscriptionService.CheckoutAsync(request);

            if (result.ResultStatus == ResultStatus.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}
