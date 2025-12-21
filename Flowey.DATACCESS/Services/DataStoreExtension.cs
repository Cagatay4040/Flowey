using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.DataAccess.Concrete;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowey.DATACCESS.Services
{
    public static class DataStoreExtension
    {
        public static IServiceCollection DataStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddDbContext<FloweyDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<AuditInterceptor>();

                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Flowey.DATACCESS"))
                    .AddInterceptors(interceptor);
            });

            services.AddScoped<DbContext>(provider => provider.GetService<FloweyDbContext>());

            return services;
        }
    }
}
