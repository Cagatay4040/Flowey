using Flowey.CORE.DataAccess.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Concrete
{
    public class RoleRepository : EfEntityRepositoryBase<Role>, IRoleRepository
    {
        private readonly FloweyDbContext _context;

        public RoleRepository(FloweyDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
