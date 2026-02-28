using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Abstract
{
    public interface IProjectRepository : IEntityRepository<Project>
    {
        #region Get Methods

        Task<List<ProjectUserRole>> GetUserProjectMembershipsAsync(Guid userId);
        Task<List<ProjectUserRole>> GetUserProjectMembershipsAsync(Guid userId, RoleType roleFilter);
        Task<ProjectUserRole> GetProjectUserAsync(Guid projectId, Guid userId);
        Task<List<ProjectUserRole>> GetProjectUsersAsync(Guid projectId);
        Task<bool> IsUserInProjectAsync(ProjectUserRole projectUserRole);

        #endregion

        #region Insert Methods

        System.Threading.Tasks.Task AddWithCreatorAsync(Project project, Guid userId);
        System.Threading.Tasks.Task AddUserToProjectAsync(ProjectUserRole projectUserRole);

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods

        System.Threading.Tasks.Task RemoveUserFromProjectAsync(ProjectUserRole projectUserRole);

        #endregion
    }
}
