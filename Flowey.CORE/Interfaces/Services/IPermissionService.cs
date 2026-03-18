using Flowey.SHARED.Enums;

namespace Flowey.CORE.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<bool> HasProjectPermissionAsync(Guid userId, Guid projectId, params RoleType[] allowedRoles);
        Task<bool> HasTaskPermissionAsync(Guid userId, Guid taskId, params RoleType[] allowedRoles);
        Task<bool> HasStepPermissionAsync(Guid userId, Guid stepId, params RoleType[] allowedRoles);
        Task<bool> HasCommentPermissionAsync(Guid userId, Guid commentId, params RoleType[] allowedRoles);
    }
}
