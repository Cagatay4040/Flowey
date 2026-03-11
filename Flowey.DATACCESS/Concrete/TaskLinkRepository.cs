using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DATACCESS.Concrete
{
    public class TaskLinkRepository : EfEntityRepositoryBase<TaskLink>, ITaskLinkRepository
    {
        public TaskLinkRepository(FloweyDbContext dbContext) : base(dbContext) { }
    }
}
