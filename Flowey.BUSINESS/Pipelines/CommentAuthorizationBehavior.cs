using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Security;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Pipelines
{
    public class CommentAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequireCommentAuthorization
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public CommentAuthorizationBehavior(ICurrentUserService currentUserService, IPermissionService permissionService)
        {
            _currentUserService = currentUserService;
            _permissionService = permissionService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            bool hasPermission = await _permissionService.HasCommentPermissionAsync(_currentUserService.GetUserId().Value, request.CommentId, request.RequiredRoles);

            if (!hasPermission)
                throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);

            return await next();
        }
    }
}
