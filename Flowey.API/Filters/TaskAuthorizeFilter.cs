using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Flowey.API.Filters
{
    public class TaskAuthorizeFilter : IAsyncActionFilter
    {
        private readonly RoleType[] _allowedRoles;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public TaskAuthorizeFilter(RoleType[] allowedRoles, ICurrentUserService currentUserService, IPermissionService permissionService, ITaskService taskService)
        {
            _allowedRoles = allowedRoles;
            _currentUserService = currentUserService;
            _permissionService = permissionService;
        }

        public async System.Threading.Tasks.Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdString = _currentUserService.GetUserId() != null ? _currentUserService.GetUserId().Value.ToString() : null;

            if (string.IsNullOrEmpty(userIdString))
            {
                context.Result = new UnauthorizedObjectResult(new Result(ResultStatus.Error, Messages.UnauthorizedAccess));
                return;
            }

            var userId = Guid.Parse(userIdString);
            Guid taskId = Guid.Empty;
            bool found = false;

            if (context.ActionArguments.TryGetValue("taskId", out var idObj) && idObj is Guid directId)
            {
                taskId = directId;
                found = true;
            }
            else
            {
                foreach (var arg in context.ActionArguments.Values)
                {
                    if (arg == null) continue;

                    var type = arg.GetType();

                    var propInfo = type.GetProperty("TaskId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (propInfo != null && propInfo.PropertyType == typeof(Guid))
                    {
                        var value = propInfo.GetValue(arg);
                        if (value is Guid guidValue && guidValue != Guid.Empty)
                        {
                            taskId = guidValue;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found || taskId == Guid.Empty)
            {
                context.Result = new BadRequestObjectResult(new Result(ResultStatus.Error, Messages.TaskIdMissing));
                return;
            }

            bool hasPermission = await _permissionService.HasTaskPermissionAsync(userId, taskId, _allowedRoles);

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
