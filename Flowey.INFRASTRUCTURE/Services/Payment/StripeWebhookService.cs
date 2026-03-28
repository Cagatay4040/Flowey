using Flowey.CORE.Events.Subscription;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Flowey.INFRASTRUCTURE.Services.Payment
{
    public class StripeWebhookService : IWebhookService
    {
        private readonly IPublisher _publisher;
        private readonly StripeSettings _stripeSettings;

        public StripeWebhookService(IPublisher publisher, IOptions<StripeSettings> stripeOptions)
        {
            _publisher = publisher;
            _stripeSettings = stripeOptions.Value;
        }

        public async Task<bool> ProcessStripeWebhookAsync(string jsonBody, string signatureHeader)
        {
            try
            {
                var endpointSecret = _stripeSettings.WebhookSecret;

                var stripeEvent = EventUtility.ConstructEvent(jsonBody, signatureHeader, endpointSecret);

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session.Metadata.TryGetValue("UserId", out string userIdString) &&
                        session.Metadata.TryGetValue("MonthsToPurchase", out string monthsString) &&
                        Guid.TryParse(userIdString, out Guid userId) &&
                        int.TryParse(monthsString, out int monthsToPurchase))
                    {
                        await _publisher.Publish(new SubscriptionPaidEvent
                        {
                            UserId = userId,
                            MonthsToPurchase = monthsToPurchase
                        });
                    }
                }

                return true;
            }
            catch (StripeException)
            {
                return false;
            }
        }
    }
}