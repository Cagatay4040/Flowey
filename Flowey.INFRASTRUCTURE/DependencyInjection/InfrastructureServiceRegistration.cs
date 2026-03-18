using Flowey.CORE.Interfaces.Services;
using Flowey.Infrastructure.Services.Security;
using Flowey.INFRASTRUCTURE.Services.Notifications;
using Flowey.INFRASTRUCTURE.Services.Storage;
using Flowey.INFRASTRUCTURE.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowey.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddScoped<IRealTimeNotificationService, SignalRNotificationManager>();
            services.AddScoped<IImageService, CloudinaryImageService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ILocalFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}