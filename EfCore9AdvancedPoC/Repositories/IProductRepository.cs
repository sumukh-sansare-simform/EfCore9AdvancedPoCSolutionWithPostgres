using EfCore9AdvancedPoCWithPostgres.Models;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
        Task<Product> AddAsync(Product product);
        Task<int> UpdateAsync(Product product);
        Task<int> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
