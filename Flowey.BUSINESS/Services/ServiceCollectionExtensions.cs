using Flowey.BUSINESS.Concrete;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.DATACCESS.Concrete;
using Flowey.DATACCESS.Services;
using Flowey.DOMAIN.Model.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            serviceCollection.AddScoped<IProjectRepository, ProjectRepository>();
            serviceCollection.AddScoped<IStepRepository, StepRepository>();
            serviceCollection.AddScoped<ITaskRepository, TaskRepository>();
            serviceCollection.AddScoped<ITaskLinkRepository, TaskLinkRepository>();
            serviceCollection.AddScoped<ICommentRepository, CommentRepository>();
            serviceCollection.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

            serviceCollection.AddScoped<IPermissionService, PermissionManager>();  

            serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
            serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            serviceCollection.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher<User>>();

            serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return serviceCollection;
        }
    }
}
