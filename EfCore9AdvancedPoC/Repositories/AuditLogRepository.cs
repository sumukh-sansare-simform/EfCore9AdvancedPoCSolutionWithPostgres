using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs.ToListAsync();
        }

        public async Task<AuditLog> GetByIdAsync(int id)
        {
            return await _context.AuditLogs.FindAsync(id);
        }

        public async Task<List<AuditLog>> FindAsync(Expression<Func<AuditLog, bool>> predicate)
        {
            return await _context.AuditLogs.Where(predicate).ToListAsync();
        }

        public async Task<AuditLog> AddAsync(AuditLog entity)
        {
            await _context.AuditLogs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(AuditLog entity)
        {
            _context.AuditLogs.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(AuditLog entity)
        {
            _context.AuditLogs.Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return 0;
            return await DeleteAsync(entity);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AuditLogs.AnyAsync(a => a.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.AuditLogs.CountAsync();
        }

        public async Task<List<AuditLog>> GetByTableNameAsync(string tableName)
        {
            return await _context.AuditLogs
                .Where(a => a.TableName == tableName)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByOperationAsync(string operation)
        {
            return await _context.AuditLogs
                .Where(a => a.Operation == operation)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.AuditLogs
                .Where(a => a.Timestamp >= start && a.Timestamp <= end)
                .ToListAsync();
        }
    }
}
