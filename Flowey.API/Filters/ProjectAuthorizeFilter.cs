using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Flowey.API.Filters
{
    public class ProjectAuthorizeFilter : IAsyncActionFilter
    {
        private readonly RoleType[] _allowedRoles;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public ProjectAuthorizeFilter(RoleType[] allowedRoles, ICurrentUserService currentUserService, IPermissionService permissionService)
        {
            _allowedRoles = allowedRoles;
            _currentUserService = currentUserService;
            _permissionService = permissionService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdString = _currentUserService.GetUserId() != null ? _currentUserService.GetUserId().Value.ToString() : null;

            if (string.IsNullOrEmpty(userIdString))
            {
                context.Result = new UnauthorizedObjectResult(new Result(ResultStatus.Error, Messages.UnauthorizedAccess));
                return;
            }

            var userId = Guid.Parse(userIdString);
            Guid projectId = Guid.Empty;
            bool found = false;

            if (context.ActionArguments.TryGetValue("projectId", out var idObj) && idObj is Guid directId)
            {
                projectId = directId;
                found = true;
            }
            else
            {
                foreach (var arg in context.ActionArguments.Values)
                {
                    if (arg == null) continue;

                    var type = arg.GetType();

                    var propInfo = type.GetProperty("ProjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (propInfo != null && propInfo.PropertyType == typeof(Guid))
                    {
                        var value = propInfo.GetValue(arg);
                        if (value is Guid guidValue && guidValue != Guid.Empty)
                        {
                            projectId = guidValue;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found || projectId == Guid.Empty)
            {
                context.Result = new BadRequestObjectResult(new Result(ResultStatus.Error, Messages.ProjectIdMissing));
                return;
            }

            bool hasPermission = await _permissionService.HasPermissionAsync(userId, projectId, _allowedRoles);

            if (!hasPermission)
            {
                context.Result = new ObjectResult(new Result(ResultStatus.Error, Messages.InsufficientPermissions))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            await next();
        }
    }
}
