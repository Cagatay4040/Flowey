using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Abstract
{
    public interface IProjectRepository : IEntityRepository<Project>
    {
        #region Get Methods

        Task<Project> GetProjectWithUsersAsync(Guid projectId, bool noTracking = false);
        Task<bool> IsUserInProjectAsync(Guid projectId, Guid userId);

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
