using Flowey.API.Filters;
using Flowey.CORE.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Attributes
{
    public class ProjectAuthorizeAttribute : TypeFilterAttribute
    {
        public ProjectAuthorizeAttribute(params RoleType[] roles) : base(typeof(ProjectAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
