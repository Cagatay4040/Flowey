using System;
using System.Threading.Tasks;

namespace Flowey.DATACCESS.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
