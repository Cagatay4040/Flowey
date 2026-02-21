using Flowey.CORE.Constants;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Flowey.API.Extensions
{
    public static class AuthRegistration
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthConfig:Secret"])),
                    NameClaimType = ClaimTypes.Name
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = new Result(
                            ResultStatus.Error,
                            AuthMessages.PremiumMembershipRequired
                        );

                        await context.Response.WriteAsJsonAsync(result);
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = new Result(
                            ResultStatus.Error,
                            AuthMessages.AuthenticationRequired
                        );

                        await context.Response.WriteAsJsonAsync(result);
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequirePremium", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var expireDateClaim = context.User.FindFirst("PremiumExpireDate");

                        if (expireDateClaim == null || string.IsNullOrEmpty(expireDateClaim.Value))
                            return false;

                        if (DateTime.TryParse(expireDateClaim.Value, out DateTime expireDate))
                            return expireDate > DateTime.UtcNow;

                        return false;
                    });
                });
            });

            return services;
        }
    }
}
