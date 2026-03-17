
using AngleSharp;
using Flowey.API.Extensions;
using Flowey.API.Hubs;
using Flowey.API.Services;
using Flowey.BUSINESS.Services;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Result.Concrete;
using Flowey.Infrastructure.DependencyInjection;
using Flowey.SHARED.Constants;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Flowey.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console()
                            .WriteTo.File("logs/flowey-log-.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();

            try
            {
                Log.Information("Flowey API starting up...");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            var errors = context.ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .SelectMany(x => x.Value.Errors.Select(e => new ValidationErrorDetail
                                {
                                    Field = x.Key,
                                    Message = e.ErrorMessage
                                }))
                                .ToList();

                            var resultModel = new ValidationResult(Messages.ValidationFailed, errors);

                            return new BadRequestObjectResult(resultModel);
                        };
                    });

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme."
                    });
                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
                });

                builder.Services.AddHttpContextAccessor();
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        builder =>
                        {
                            builder.WithOrigins("http://localhost:3000")
                                   .AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowCredentials();
                        });
                });

                builder.Services.AddSignalR();

                builder.Services.ConfigureAuth(builder.Configuration);
                builder.Services.AddMyServices(builder.Configuration);
                builder.Services.AddInfrastructureServices(builder.Configuration);
                builder.Services.AddFluentValidationAutoValidation();

                builder.Services.AddScoped<IRealTimeNotificationService, SignalRNotificationManager>();

                var app = builder.Build();

                app.UseExceptionHandler(opt => { });

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseStaticFiles();
                app.UseCors("AllowAll");

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.MapHub<NotificationHub>("/hubs/notification");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start correctly.");
            }
            finally 
            {
                Log.CloseAndFlush();
            }
        }
    }
}
