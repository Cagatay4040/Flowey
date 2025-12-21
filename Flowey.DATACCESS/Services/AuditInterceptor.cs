using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Flowey.CORE.DataAccess.Abstract;

namespace Flowey.DATACCESS.Services
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.IsActive = true;
                    entry.Entity.CreatedBy = _currentUserService.GetUserId().Value;
                    entry.Entity.CreatedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedBy = _currentUserService.GetUserId().Value;
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.Entity.IsActive = false;
                    entry.Entity.ModifiedBy = _currentUserService.GetUserId().Value;
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
