using Flowey.BUSINESS.Abstract;
using Flowey.CORE.Enums;
using Flowey.DATACCESS.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Concrete
{
    public class PermissionManager : IPermissionService
    {
        private readonly IProjectRepository _projectRepository;

        public PermissionManager(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, Guid projectId, params RoleType[] allowedRoles)
        {
            var projectUser = await _projectRepository.GetProjectUserAsync(projectId, userId);

            if (projectUser == null)
                return false;

            if (allowedRoles == null || !allowedRoles.Any())
                return true;

            return allowedRoles.Contains((RoleType)projectUser.RoleId);
        }
    }
}
