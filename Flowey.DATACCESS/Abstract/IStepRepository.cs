using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Abstract
{
    public interface IStepRepository : IEntityRepository<Step> 
    {
        #region Get Methods

        Task<List<Step>> GetProjectStepsAsync(Guid projectId);

        #endregion

        #region Insert Methods

        Task<int> AddStepToProjectAsync(Step step, Guid projectId);
        Task<int> AddRangeStepToProjectAsync(List<Step> steps, Guid projectId);

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
