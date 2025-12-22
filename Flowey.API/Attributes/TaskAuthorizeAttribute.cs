using Flowey.API.Filters;
using Flowey.CORE.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Attributes
{
    public class TaskAuthorizeAttribute : TypeFilterAttribute
    {
        public TaskAuthorizeAttribute(params RoleType[] roles) : base(typeof(TaskAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
