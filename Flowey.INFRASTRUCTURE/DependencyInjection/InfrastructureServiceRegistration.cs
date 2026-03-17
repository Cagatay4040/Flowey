using Flowey.CORE.Interfaces.Services;
using Flowey.Infrastructure.Services.Security;
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

            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }
    }
}