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
            var data = await _context.ProjectSteps
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .Select(x => x.Step)
                .ToListAsync();

            return data;
        }

        #endregion

        #region Insert Methods

        public async Task<int> AddStepToProjectAsync(Step step, Guid projectId)
        {
            await _context.Steps.AddAsync(step);
            await _context.ProjectSteps.AddAsync(new ProjectStep
            {
                ProjectId = projectId,
                StepId = step.Id
            });
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddRangeStepToProjectAsync(List<Step> steps, Guid projectId)
        {
            await _context.Steps.AddRangeAsync(steps);

            var projectSteps = steps.Select(x => new ProjectStep
            {
                Step = x,
                ProjectId = projectId
            }).ToList();

            await _context.ProjectSteps.AddRangeAsync(projectSteps);
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
