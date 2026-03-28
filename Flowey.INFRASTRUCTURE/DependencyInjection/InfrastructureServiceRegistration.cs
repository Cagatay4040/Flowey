using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Settings;
using Flowey.Infrastructure.Services.Security;
using Flowey.INFRASTRUCTURE.Services.Notifications;
using Flowey.INFRASTRUCTURE.Services.Payment;
using Flowey.INFRASTRUCTURE.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace Flowey.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

            var stripeSettings = configuration.GetSection("Stripe").Get<StripeSettings>();
            StripeConfiguration.ApiKey = stripeSettings.SecretKey;

            services.AddScoped<IRealTimeNotificationService, SignalRNotificationManager>();
            services.AddScoped<IImageService, CloudinaryImageService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ILocalFileStorageService, LocalFileStorageService>();
            services.AddScoped<IWebhookService, StripeWebhookService>();
            services.AddScoped<IPaymentService, StripePaymentService>();

            return services;
        }
    }
}