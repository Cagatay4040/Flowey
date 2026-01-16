using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Collections;

namespace Flowey.API.Filters
{
    public class CommentAuthorizeFilter : IAsyncActionFilter
    {
        private readonly RoleType[] _allowedRoles;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public CommentAuthorizeFilter(RoleType[] allowedRoles, ICurrentUserService currentUserService, IPermissionService permissionService)
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
            Guid commentId = Guid.Empty;
            bool found = false;

            if (context.ActionArguments.TryGetValue("commentId", out var idObj) && idObj is Guid directId)
            {
                commentId = directId;
            }
            else if (context.ActionArguments.TryGetValue("taskId", out var _idObj) && _idObj is Guid _directId)
            {
                taskId = _directId;
            }
            else
            {
                foreach (var arg in context.ActionArguments.Values)
                {
                    if (arg == null) continue;

                    if (arg is IEnumerable listArg && !(arg is string))
                    {
                        foreach (var item in listArg)
                        {
                            if (item == null) continue;
                            CheckProperties(item, ref taskId, ref commentId);

                            if (taskId != Guid.Empty) break;
                        }
                    }
                    else
                    {
                        CheckProperties(arg, ref taskId, ref commentId);
                    }

                    if (taskId != Guid.Empty) break;
                    if (commentId != Guid.Empty) break;
                }
            }

            bool hasPermission = false;

            if (taskId != Guid.Empty)
            {
                hasPermission = await _permissionService.HasTaskPermissionAsync(userId, taskId, _allowedRoles);
            }
            else if (commentId != Guid.Empty)
            {
                hasPermission = await _permissionService.HasCommentPermissionAsync(userId, commentId, _allowedRoles);
            }
            else
            {
                context.Result = new ObjectResult(new Result(ResultStatus.Error, Messages.CommentIdOrTaskIdMissing))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

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

        void CheckProperties(object obj, ref Guid taskId, ref Guid commentId)
        {
            var type = obj.GetType();

            if (taskId == Guid.Empty)
            {
                var taskProp = type.GetProperty("TaskId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (taskProp != null && taskProp.PropertyType == typeof(Guid))
                {
                    var val = taskProp.GetValue(obj);
                    if (val is Guid g && g != Guid.Empty) taskId = g;
                }
            }

            if (commentId == Guid.Empty)
            {
                var commentProp = type.GetProperty("CommentId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (commentProp != null && commentProp.PropertyType == typeof(Guid))
                {
                    var val = commentProp.GetValue(obj);
                    if (val is Guid g && g != Guid.Empty) commentId = g;
                }
            }
        }
    }
}
