using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.SHARED.Constants
{
    public static class AuthMessages
    {
        public const string AuthenticationRequired = "Authentication is required to access this resource. Please log in.";

        public const string PremiumMembershipRequired = "Access denied. Premium membership is required to perform this action.";

        public const string JwtSettingsNotFound = "JWT settings could not be found in the configuration file.";
    }
}
