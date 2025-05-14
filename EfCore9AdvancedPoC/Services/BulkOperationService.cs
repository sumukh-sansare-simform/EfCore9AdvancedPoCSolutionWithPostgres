using EfCore9AdvancedPoCWithPostgres.Models;
using EfCore9AdvancedPoCWithPostgres.Repositories;

namespace EfCore9AdvancedPoCWithPostgres.Services
{
    public class BulkOperationService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BulkOperationService> _logger;

        public BulkOperationService(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            ILogger<BulkOperationService> logger)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> CheckDatabaseHealthAsync()
        {
            try
            {
                var userCount = await _userRepository.CountAsync();
                var productCount = await _productRepository.CountAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return false;
            }
        }

        // Bulk update product prices by percentage
        public async Task<int> UpdateProductPricesAsync(decimal percentage)
        {
            var products = await _productRepository.GetAllAsync();
            foreach (var product in products)
            {
                product.Price *= (1 + percentage / 100);
                product.UpdatedAt = DateTime.UtcNow;
            }

            int updatedCount = 0;
            foreach (var product in products)
            {
                await _productRepository.UpdateAsync(product);
                updatedCount++;
            }

            return updatedCount;
        }

        // Bulk delete old orders
        public async Task<int> DeleteOldOrdersAsync(DateTime cutoffDate)
        {
            return await _orderRepository.DeleteOldOrdersAsync(cutoffDate);
        }

        // Opt-in all users to newsletter
        public async Task<int> OptInAllUsersToNewsletterAsync()
        {
            _logger.LogInformation("Starting bulk opt-in for newsletter");
            return await _userRepository.OptInAllToNewsletterAsync();
        }

        // Batch update product inventory
        public async Task<int> BatchUpdateProductInventoryAsync(Dictionary<int, int> productQuantityChanges)
        {
            int totalUpdated = 0;

            foreach (var batch in productQuantityChanges.Chunk(100))
            {
                var productIds = batch.Select(x => x.Key).ToList();
                var products = await _productRepository.FindAsync(p => productIds.Contains(p.Id));

                foreach (var product in products)
                {
                    if (productQuantityChanges.TryGetValue(product.Id, out int change))
                    {
                        product.Quantity += change;
                        product.UpdatedAt = DateTime.UtcNow;
                        await _productRepository.UpdateAsync(product);
                        totalUpdated++;
                    }
                }
            }

            return totalUpdated;
        }

        // Get products with full details
        public async Task<List<Product>> GetProductsWithFullDetailsAsync(
            bool includeInventoryDetails = false,
            bool includeSalesHistory = false)
        {
            if (includeInventoryDetails)
            {
                return await _productRepository.GetProductsWithDetailsAsync();
            }
            
            if (includeSalesHistory)
            {
                return await _productRepository.GetProductsWithTagsAsync();
            }
            
            return await _productRepository.GetAllAsync();
        }

        // Get paginated products
        public async Task<(List<Product> Items, int TotalCount)> GetPaginatedProductsAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var products = await _productRepository.GetAllAsync();
            
            var totalCount = products.Count;
            var pagedItems = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedItems, totalCount);
        }
    }
}
