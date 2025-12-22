using Flowey.API.Filters;
using Flowey.CORE.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Attributes
{
    public class StepAuthorizeAttribute : TypeFilterAttribute
    {
        public StepAuthorizeAttribute(params RoleType[] roles) : base(typeof(StepAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
