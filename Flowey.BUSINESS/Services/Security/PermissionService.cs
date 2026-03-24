using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Enums;

namespace Flowey.BUSINESS.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IStepRepository _stepRepository;
        private readonly ICommentRepository _commentRepository;

        public PermissionService(IEntityRepository<ProjectUserRole> projectUserRoleRepository, ITaskRepository taskRepository, IStepRepository stepRepository, ICommentRepository commentRepository)
        {
            _projectUserRoleRepository = projectUserRoleRepository;
            _taskRepository = taskRepository;
            _stepRepository = stepRepository;
            _commentRepository = commentRepository;
        }

        public async Task<bool> HasProjectPermissionAsync(Guid userId, Guid projectId, params RoleType[] allowedRoles)
        {
            var projectUser = await _projectUserRoleRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

            if (projectUser == null)
                return false;

            if (allowedRoles == null || !allowedRoles.Any())
                return true;

            return allowedRoles.Contains(projectUser.RoleId);
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

        public async Task<bool> HasCommentPermissionAsync(Guid userId, Guid commentId, params RoleType[] allowedRoles)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, includes: x => x.Task);

            if (comment == null)
                return false;

            return await HasTaskPermissionAsync(userId, comment.Task.Id, allowedRoles);
        }
    }
}
