using Flowey.API.Filters;
using Flowey.CORE.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Flowey.API.Attributes
{
    public class CommentAuthorizeAttribute : TypeFilterAttribute
    {
        public CommentAuthorizeAttribute(params RoleType[] roles) : base(typeof(CommentAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
