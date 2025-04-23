using EfCore9AdvancedPoC.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EfCore9AdvancedPoCWithPostgres.Models;

namespace EfCore9AdvancedPoCWithPostgres.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var logs = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            {
                logs.Add(new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Operation = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow
                });
            }

            context.Set<AuditLog>().AddRange(logs);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

}
