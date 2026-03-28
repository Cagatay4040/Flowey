namespace Flowey.CORE.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> CreateCheckoutSessionAsync(Guid userId, int monthsToPurchase);
    }
}
