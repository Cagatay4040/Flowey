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

        Task<int> AddAndAssignTaskAsync(Task task, Guid userId, Guid stepId);

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
