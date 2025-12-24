using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Abstract
{
    public interface IStepRepository : IEntityRepository<Step> 
    {
        #region Get Methods

        Task<List<Step>> GetProjectStepsAsync(Guid projectId);
        Task<Step> GetProjectFirstStepAsync(Guid projectId);
        Task<List<Step>> GetStepsWithFilteredTasksAsync(Guid projectId, List<Guid> userIds);

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods

        Task<int> SoftDeleteAndReOrderStepsAsync(Step step);

        #endregion
    }
}
