using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Abstract
{
    public interface IProjectRepository : IEntityRepository<Project>
    {
        #region Get Methods

        Task<List<Project>> GetProjectsByLoginUserAsync(Guid userId);
        Task<ProjectUserRole> GetProjectUserAsync(Guid projectId, Guid userId);
        Task<List<ProjectUserRole>> GetProjectUsersAsync(Guid projectId);
        Task<bool> IsUserInProjectAsync(ProjectUserRole projectUserRole);

        #endregion

        #region Insert Methods

        Task<int> AddWithCreatorAsync(Project project, Guid userId);
        Task<int> AddUserToProjectAsync(ProjectUserRole projectUserRole);

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods

        Task<int> RemoveUserFromProjectAsync(ProjectUserRole projectUserRole);

        #endregion
    }
}
