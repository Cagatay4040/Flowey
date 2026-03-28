namespace Flowey.CORE.Interfaces.Services
{
    public interface IWebhookService
    {
        Task<bool> ProcessStripeWebhookAsync(string jsonBody, string signatureHeader);
    }
}