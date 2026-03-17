using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.SHARED.Constants
{
    public static class ExceptionMessages
    {
        public const string InternalServerError = "An unexpected system error occurred. Please try again later.";
        public const string NotFound = "The requested resource could not be found.";
        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string ValidationFailed = "One or more validation errors occurred.";
    }
}
