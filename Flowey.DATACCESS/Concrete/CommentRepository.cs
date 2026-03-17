using Flowey.CORE.Interfaces.Repositories;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.DATACCESS.Concrete
{
    public class CommentRepository : EfEntityRepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(FloweyDbContext dbContext) : base(dbContext) { }
    }
}
