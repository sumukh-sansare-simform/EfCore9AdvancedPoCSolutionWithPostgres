using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<List<Order>> FindAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders.Where(predicate).ToListAsync();
        }

        public async Task<Order> AddAsync(Order entity)
        {
            await _context.Orders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(Order entity)
        {
            _context.Orders.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Order entity)
        {
            _context.Orders.Remove(entity);
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
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<List<Order>> GetOrdersByProductNameAsync(string productName)
        {
            return await _context.Orders
                .Where(o => o.Details.ProductName == productName)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersWithTagAsync(string tag)
        {
            return await _context.Orders
                .Where(o => EF.Functions.JsonContains(
                    EF.Property<string>(o, "Details"),
                    $@"{{""Tags"": [""{tag}""]}}"
                ))
                .ToListAsync();
        }

        public async Task<int> DeleteOldOrdersAsync(DateTime cutoffDate)
        {
            return await _context.Orders
                .Where(o => o.OrderedAt < cutoffDate)
                .ExecuteDeleteAsync();
        }
    }
}
