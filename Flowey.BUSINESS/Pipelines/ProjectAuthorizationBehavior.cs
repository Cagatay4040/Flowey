using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Security;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Pipelines
{
    public class ProjectAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequireProjectAuthorization
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public ProjectAuthorizationBehavior(ICurrentUserService currentUserService, IPermissionService permissionService)
        {
            _currentUserService = currentUserService;
            _permissionService = permissionService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            bool hasPermission = await _permissionService.HasProjectPermissionAsync(_currentUserService.GetUserId().Value, request.ProjectId, request.RequiredRoles);

            if (!hasPermission)
                throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);

            return await next();
        }
    }
}
