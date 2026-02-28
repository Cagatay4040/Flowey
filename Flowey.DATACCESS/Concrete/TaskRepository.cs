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



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
