using Flowey.CORE.DataAccess.Abstract;
using Flowey.SHARED.Enums;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Flowey.CORE.Interfaces.Services;

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
            var userId = _currentUserService.GetUserId().Value;
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

            bool hasPermission = await _permissionService.HasProjectPermissionAsync(userId, projectId, _allowedRoles);

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
