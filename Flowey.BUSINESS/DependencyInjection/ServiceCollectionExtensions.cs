using Flowey.BUSINESS.Pipelines;
using Flowey.BUSINESS.Services.Security;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.DATACCESS.Concrete;
using Flowey.DATACCESS.DependencyInjection;
using Flowey.DOMAIN.Model.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flowey.BUSINESS.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.DataStore(configuration);

            services.AddScoped(typeof(IEntityRepository<>), typeof(EfEntityRepositoryBase<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IStepRepository, StepRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskLinkRepository, TaskLinkRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPermissionService, PermissionService>();  

            services.AddAutoMapper(typeof(ServiceCollectionExtensions));
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                cfg.AddOpenBehavior(typeof(TaskAuthorizationBehavior<,>));
                cfg.AddOpenBehavior(typeof(CommentAuthorizationBehavior<,>));
                cfg.AddOpenBehavior(typeof(ProjectAuthorizationBehavior<,>));
                cfg.AddOpenBehavior(typeof(StepAuthorizationBehavior<,>));
            });

            services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher<User>>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
