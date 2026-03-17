using Flowey.CORE.Interfaces.Repositories;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
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



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
