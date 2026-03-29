using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.Services;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Pipelines
{
    public class TaskAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequireTaskAuthorization
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public TaskAuthorizationBehavior(ICurrentUserService currentUserService, IPermissionService permissionService)
        {
            _currentUserService = currentUserService;
            _permissionService = permissionService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            bool hasPermission = await _permissionService.HasTaskPermissionAsync(_currentUserService.GetUserId().Value, request.TaskId, request.RequiredRoles);

            if (!hasPermission)
                throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);

            return await next();
        }
    }
}