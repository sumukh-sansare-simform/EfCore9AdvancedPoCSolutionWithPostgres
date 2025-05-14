using EfCore9AdvancedPoCWithPostgres.Models;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrdersByProductNameAsync(string productName);
        Task<List<Order>> GetOrdersWithTagAsync(string tag);
        Task<int> DeleteOldOrdersAsync(DateTime cutoffDate);
    }
}
