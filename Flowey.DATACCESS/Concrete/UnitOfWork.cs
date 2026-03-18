using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;

namespace Flowey.DATACCESS.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FloweyDbContext _context;

        public UnitOfWork(FloweyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
