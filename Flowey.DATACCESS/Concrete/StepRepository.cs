using Flowey.CORE.DataAccess.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Flowey.DATACCESS.Concrete
{
    public class StepRepository : EfEntityRepositoryBase<Step>, IStepRepository
    {
        private readonly FloweyDbContext _context;

        public StepRepository(FloweyDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        #region Get Methods

        public async Task<List<Step>> GetProjectStepsAsync(Guid projectId)
        {
            var data = await _context.Steps
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.Order)
                .ToListAsync();

            return data;
        }

        public async Task<Step> GetProjectFirstStepAsync(Guid projectId)
        {
            var data = await _context.Steps
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.Order)
                .FirstOrDefaultAsync();

            return data;
        }

        public async Task<List<Step>> GetStepsWithFilteredTasksAsync(Guid projectId, List<Guid> userIds, bool includeUnassigned)
        {
            var targetUserIds = userIds ?? new List<Guid>();
            bool hasUsers = targetUserIds.Any();

            var query = _context.Steps
                                .AsNoTracking()
                                .Where(s => s.ProjectId == projectId)
                                .OrderBy(s => s.Order)
                                .AsQueryable();

            query = query.Include(s => s.Tasks
                         .Where(t =>
                             (hasUsers && t.AssigneeId != null && targetUserIds.Contains(t.AssigneeId.Value))
                             ||
                             (includeUnassigned && t.AssigneeId == null)
                         )
                         .OrderBy(t => t.CreatedDate));

            return await query.ToListAsync();
        }

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods

        public async Task<int> SoftDeleteAndReOrderStepsAsync(Step step)
        {
            var stepsToReorder = await _context.Steps
                .Where(s => s.ProjectId == step.ProjectId && s.Order > step.Order)
                .ToListAsync();

            foreach (var s in stepsToReorder)
            {
                s.Order -= 1;
            }

            _context.Steps.Remove(step);

            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}
