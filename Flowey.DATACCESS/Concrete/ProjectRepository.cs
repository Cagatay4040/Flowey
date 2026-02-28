using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Flowey.DATACCESS.Concrete
{
    public class ProjectRepository : EfEntityRepositoryBase<Project>, IProjectRepository
    {
        private readonly FloweyDbContext _context;

        public ProjectRepository(FloweyDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        #region Get Methods

        public async Task<Project> GetProjectWithUsersAsync(Guid projectId, bool noTracking = false)
        {
            var query = _context.Projects             
                    .Include(p => p.ProjectUserRoles)
                    .ThenInclude(pur => pur.User)
                    .AsQueryable();

            if (noTracking)
                query = query.AsNoTracking();

            var data = await query.FirstOrDefaultAsync(p => p.Id == projectId);  

            return data;
        }

        public async Task<bool> IsUserInProjectAsync(Guid projectId, Guid userId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId &&
                               p.ProjectUserRoles.Any(ur => ur.UserId == userId));
        }

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
