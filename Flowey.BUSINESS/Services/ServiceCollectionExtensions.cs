using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete;
using Flowey.DATACCESS.Services;
using Flowey.DOMAIN.Model.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddExceptionHandler<GlobalExceptionHandler>();
            serviceCollection.AddProblemDetails();

            serviceCollection.DataStore(configuration);

            serviceCollection.AddScoped(typeof(IEntityRepository<>), typeof(EfEntityRepositoryBase<>));
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IRoleRepository, RoleRepository>();
            serviceCollection.AddScoped<IProjectRepository, ProjectRepository>();
            serviceCollection.AddScoped<IStepRepository, StepRepository>();
            serviceCollection.AddScoped<ITaskRepository, TaskRepository>();
            serviceCollection.AddScoped<ICommentRepository, CommentRepository>();
            serviceCollection.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

            serviceCollection.AddScoped<IAuthService, AuthManager>();
            serviceCollection.AddScoped<IRoleService, RoleManager>();
            serviceCollection.AddScoped<IUserService, UserManager>();
            serviceCollection.AddScoped<ICommentService, CommentManager>();
            serviceCollection.AddScoped<IPermissionService, PermissionManager>();
            serviceCollection.AddScoped<IFileService, FileManager>();
            serviceCollection.AddScoped<ISubscriptionService, SubscriptionManager>();
            serviceCollection.AddScoped<IUserNotificationService, UserNotificationManager>();

            serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
            serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            serviceCollection.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher<User>>();

            serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return serviceCollection;
        }
    }
}
