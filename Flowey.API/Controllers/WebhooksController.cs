using Flowey.CORE.Interfaces.Services;
using Flowey.SHARED.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly IWebhookService _webhookService;

        public WebhooksController(IWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signatureHeader = Request.Headers["Stripe-Signature"];

            bool isSignatureValid = await _webhookService.ProcessStripeWebhookAsync(json, signatureHeader);

            if (!isSignatureValid)
                return BadRequest(Messages.InvalidWebhookSignature);

            return Ok();
        }
    }
}