using Flowey.BUSINESS.Abstract;
using Flowey.CORE.Enums;
using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete;
using Flowey.DOMAIN.Model.Concrete;
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
        private readonly ITaskRepository _taskRepository;
        private readonly IStepRepository _stepRepository;

        public PermissionManager(IProjectRepository projectRepository, ITaskRepository taskRepository, IStepRepository stepRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _stepRepository = stepRepository;
        }

        public async Task<bool> HasProjectPermissionAsync(Guid userId, Guid projectId, params RoleType[] allowedRoles)
        {
            var projectUser = await _projectRepository.GetProjectUserAsync(projectId, userId);

            if (projectUser == null)
                return false;

            if (allowedRoles == null || !allowedRoles.Any())
                return true;

            return allowedRoles.Contains((RoleType)projectUser.RoleId);
        }

        public async Task<bool> HasTaskPermissionAsync(Guid userId, Guid taskId, params RoleType[] allowedRoles)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null) 
                return false;

            return await HasProjectPermissionAsync(userId, task.ProjectId, allowedRoles);
        }

        public async Task<bool> HasStepPermissionAsync(Guid userId, Guid stepId, params RoleType[] allowedRoles)
        {
            var step = await _stepRepository.GetByIdAsync(stepId);

            if (step == null) 
                return false;

            return await HasProjectPermissionAsync(userId, step.ProjectId, allowedRoles);
        }
    }
}
