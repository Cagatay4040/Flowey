using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;
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

        public async Task<int> ChangeAssignTaskAsync(Task task, Guid userId)
        {
            task.AssigneeId = userId;

            _context.Tasks.Attach(task);
            _context.Entry(task).State = EntityState.Modified;

            await _context.TaskHistories.AddAsync(new TaskHistory
            {
                TaskId = task.Id,
                UserId = userId,
                StepId = task.CurrentStepId
            });

            return await _context.SaveChangesAsync();
        }

        public async Task<int> ChangeStepTaskAsync(Task task, Guid newStepId)
        {
            task.CurrentStepId = newStepId;

            _context.Tasks.Attach(task);
            _context.Entry(task).State = EntityState.Modified;

            await _context.TaskHistories.AddAsync(new TaskHistory
            {
                TaskId = task.Id,
                UserId = task.AssigneeId.Value,
                StepId = task.CurrentStepId
            });

            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Delete Methods



        #endregion
    }
}
