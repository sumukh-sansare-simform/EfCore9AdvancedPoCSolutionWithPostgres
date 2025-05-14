using EfCore9AdvancedPoCWithPostgres.Models;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> FindAsync(Expression<Func<Product, bool>> predicate);
        Task<List<Product>> GetProductsWithDetailsAsync();
        Task<List<Product>> GetProductsWithTagsAsync();
        Task<List<Product>> GetRecentlyViewedProductsAsync(TimeSpan timeSpan);
        Task SetLastViewedAtAsync(int productId);
        Task<Product> AddAsync(Product product);
        Task<int> UpdateAsync(Product product);
        Task<int> DeleteAsync(Product entity);
        Task<int> DeleteByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
    }
}
