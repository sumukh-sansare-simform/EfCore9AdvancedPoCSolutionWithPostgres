using EfCore9AdvancedPoCWithPostgres.Models;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<List<AuditLog>> GetByTableNameAsync(string tableName);
        Task<List<AuditLog>> GetByOperationAsync(string operation);
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}
