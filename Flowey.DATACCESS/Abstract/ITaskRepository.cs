using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Task = Flowey.DOMAIN.Model.Concrete.Task;

namespace Flowey.DATACCESS.Abstract
{
    public interface ITaskRepository : IEntityRepository<Task> 
    {
        #region Get Methods

        Task<List<TaskHistory>> GetTaskHistoryAsync(Guid taskId);

        #endregion

        #region Insert Methods

        System.Threading.Tasks.Task AddAndAssignTaskAsync(Task task, Guid userId);

        #endregion

        #region Update Methods

        System.Threading.Tasks.Task ChangeAssignTaskAsync(Task task, Guid userId);
        System.Threading.Tasks.Task ChangeStepTaskAsync(Task task, Guid newStepId);

        #endregion

        #region Delete Methods



        #endregion
    }
}
