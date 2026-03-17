using Flowey.CORE.Interfaces.Repositories;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Concrete
{
    public class TaskLinkRepository : EfEntityRepositoryBase<TaskLink>, ITaskLinkRepository
    {
        public TaskLinkRepository(FloweyDbContext dbContext) : base(dbContext) { }
    }
}
