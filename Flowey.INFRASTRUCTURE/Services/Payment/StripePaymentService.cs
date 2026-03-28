using Flowey.CORE.Interfaces.Services;
using Stripe.Checkout;

namespace Flowey.INFRASTRUCTURE.Services.Payment
{
    public class StripePaymentService : IPaymentService
    {
        public async Task<string> CreateCheckoutSessionAsync(Guid userId, int monthsToPurchase)
        {
            long unitAmountInCents = (long)(299.90m * monthsToPurchase * 100);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = unitAmountInCents,
                            Currency = "try",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Flowey Premium ({monthsToPurchase} Month)",
                                Description = "Flowey project management system premium membership."
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",

                SuccessUrl = "http://localhost:3000/payment-success",
                CancelUrl = "http://localhost:3000/payment-cancelled",

                Metadata = new Dictionary<string, string>
                {
                    { "UserId", userId.ToString() },
                    { "MonthsToPurchase", monthsToPurchase.ToString() }
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return session.Url;
        }
    }
}