using Flowey.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Abstract
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, Guid projectId, params RoleType[] allowedRoles);
    }
}
