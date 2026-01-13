using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Task = Flowey.DOMAIN.Model.Concrete.Task;

namespace Flowey.DATACCESS.Abstract
{
    public interface ITaskRepository : IEntityRepository<Task> 
    {
        #region Get Methods



        #endregion

        #region Insert Methods

        Task<int> AddAndAssignTaskAsync(Task task, Guid userId);

        #endregion

        #region Update Methods

        Task<int> ChangeAssignTaskAsync(Task task, Guid userId);
        Task<int> ChangeStepTaskAsync(Task task, Guid newStepId);

        #endregion

        #region Delete Methods



        #endregion
    }
}
