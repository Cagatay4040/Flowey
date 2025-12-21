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
                .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

            return data;
        }

        public async Task<bool> IsUserInProjectAsync(ProjectUserRole projectUserRole)
        {
            return await _context.Set<ProjectUserRole>()
                .AnyAsync(x => x.ProjectId == projectUserRole.ProjectId && x.UserId == projectUserRole.UserId);
        }

        public async Task<List<Project>> GetProjectsByLoginUserAsync(Guid userId)
        {
            var data = await _context.ProjectUserRoles
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.Project)
                .ToListAsync();

            return data;
        }

        #endregion

        #region Insert Methods

        public async Task<int> AddWithCreatorAsync(Project project, Guid userId)
        {
            await _context.Projects.AddAsync(project);
            await _context.ProjectUserRoles.AddAsync(new ProjectUserRole
            {
                ProjectId = project.Id,
                UserId = userId,
                RoleId = (int)RoleType.Admin
            });
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddUserToProjectAsync(ProjectUserRole projectUserRole)
        {
            await _context.ProjectUserRoles.AddAsync(projectUserRole);
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods

        public async Task<int> RemoveUserFromProjectAsync(ProjectUserRole projectUserRole)
        {
            var data = await GetProjectUserAsync(projectUserRole.ProjectId, projectUserRole.UserId);

            if (data != null)
            {
                data.IsActive = false;

                _context.Set<ProjectUserRole>().Attach(data);
                _context.Entry(projectUserRole).State = EntityState.Modified;
                return await _context.SaveChangesAsync();
            }

            return 0;
        }

        #endregion
    }
}
