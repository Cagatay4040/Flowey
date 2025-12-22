using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Collections;

namespace Flowey.API.Filters
{
    public class StepAuthorizeFilter : IAsyncActionFilter
    {
        private readonly RoleType[] _allowedRoles;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionService _permissionService;

        public StepAuthorizeFilter(RoleType[] allowedRoles, ICurrentUserService currentUserService, IPermissionService permissionService, ITaskService taskService)
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
            Guid stepId = Guid.Empty;
            Guid projectId = Guid.Empty;

            if (context.ActionArguments.TryGetValue("stepId", out var idObj) && idObj is Guid directId)
            {
                stepId = directId;
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
                            CheckProperties(item, ref stepId, ref projectId);

                            if (stepId != Guid.Empty) break;
                        }
                    }
                    else
                    {
                        CheckProperties(arg, ref stepId, ref projectId);
                    }

                    if (stepId != Guid.Empty) break;
                }
            }

            bool hasPermission = false;

            if (stepId != Guid.Empty)
            {
                hasPermission = await _permissionService.HasStepPermissionAsync(userId, stepId, _allowedRoles);
            }
            else if (projectId != Guid.Empty)
            {
                hasPermission = await _permissionService.HasProjectPermissionAsync(userId, projectId, _allowedRoles);
            }
            else
            {
                context.Result = new ObjectResult(new Result(ResultStatus.Error, Messages.ProjectIdOrStepIdMissing))
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

        void CheckProperties(object obj, ref Guid stepId, ref Guid projectId)
        {
            var type = obj.GetType();

            if (stepId == Guid.Empty)
            {
                var stepProp = type.GetProperty("StepId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (stepProp != null && stepProp.PropertyType == typeof(Guid))
                {
                    var val = stepProp.GetValue(obj);
                    if (val is Guid g && g != Guid.Empty) stepId = g;
                }
            }

            if (projectId == Guid.Empty) 
            {
                var projProp = type.GetProperty("ProjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (projProp != null && projProp.PropertyType == typeof(Guid))
                {
                    var val = projProp.GetValue(obj);
                    if (val is Guid g && g != Guid.Empty) projectId = g;
                }
            }
        }
    }
}
