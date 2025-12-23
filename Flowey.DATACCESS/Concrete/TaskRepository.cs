using Flowey.CORE.DataAccess.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;
using Task = Flowey.DOMAIN.Model.Concrete.Task;

namespace Flowey.DATACCESS.Concrete
{
    public class TaskRepository : EfEntityRepositoryBase<Task>, ITaskRepository
    {
        private readonly FloweyDbContext _context;

        public TaskRepository(FloweyDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        #region Get Methods



        #endregion

        #region Insert Methods

        public async Task<int> AddAndAssignTaskAsync(Task task, Guid userId)
        {
            await _context.Tasks.AddAsync(task);
            await _context.TaskHistories.AddAsync(new TaskHistory
            {
                TaskId = task.Id,
                UserId = userId,
                StepId = task.CurrentStepId
            });

            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Update Methods

        public async Task<int> ChangeAssignTaskAsync(Guid taskId, Guid userId, Guid stepId)
        {
            await _context.TaskHistories.AddAsync(new TaskHistory
            {
                TaskId = taskId,
                UserId = userId,
                StepId = stepId
            });

            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Delete Methods



        #endregion
    }
}
