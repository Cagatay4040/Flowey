using Flowey.CORE.DataAccess.Concrete;
using Flowey.CORE.Enums;
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

        public async Task<ProjectUserRole> GetProjectUserAsync(Guid projectId, Guid userId)
        {
            var data = await _context.ProjectUserRoles
                .AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

            return data;
        }

        public async Task<List<ProjectUserRole>> GetProjectUsersAsync(Guid projectId)
        {
            var data = await _context.ProjectUserRoles
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.User.Email)
                .ToListAsync();

            return data;
        }

        public async Task<bool> IsUserInProjectAsync(ProjectUserRole projectUserRole)
        {
            return await _context.Set<ProjectUserRole>()
                .AnyAsync(x => x.ProjectId == projectUserRole.ProjectId && x.UserId == projectUserRole.UserId);
        }

        private IQueryable<ProjectUserRole> BuildBaseUserProjectQuery(Guid userId)
        {
            return _context.ProjectUserRoles
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Role)
                .Where(x => x.UserId == userId);
        }

        public async Task<List<ProjectUserRole>> GetUserProjectMembershipsAsync(Guid userId)
        {
            var data = await BuildBaseUserProjectQuery(userId).ToListAsync();
              
            return data;
        }

        public async Task<List<ProjectUserRole>> GetUserProjectMembershipsAsync(Guid userId, RoleType roleFilter)
        {
            var data = await BuildBaseUserProjectQuery(userId)
                .Where(x => x.RoleId == (int)roleFilter)
                .ToListAsync();

            return data;
        }

        #endregion

        #region Insert Methods

        public async System.Threading.Tasks.Task AddWithCreatorAsync(Project project, Guid userId)
        {
            await _context.Projects.AddAsync(project);
            await _context.ProjectUserRoles.AddAsync(new ProjectUserRole
            {
                ProjectId = project.Id,
                UserId = userId,
                RoleId = (int)RoleType.Admin
            });
        }

        public async System.Threading.Tasks.Task AddUserToProjectAsync(ProjectUserRole projectUserRole)
        {
            await _context.ProjectUserRoles.AddAsync(projectUserRole);
        }

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods

        public async System.Threading.Tasks.Task RemoveUserFromProjectAsync(ProjectUserRole projectUserRole)
        {
            var data = await GetProjectUserAsync(projectUserRole.ProjectId, projectUserRole.UserId);

            if (data != null)
            {
                data.IsActive = false;

                _context.Set<ProjectUserRole>().Attach(data);
                _context.Entry(projectUserRole).State = EntityState.Modified;
            }
        }

        #endregion
    }
}
