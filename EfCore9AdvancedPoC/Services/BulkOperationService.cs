using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCore9AdvancedPoCWithPostgres.Services
{
    public class BulkOperationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BulkOperationService> _logger;

        public BulkOperationService(
       AppDbContext context,
       ILogger<BulkOperationService> logger)
        {
            _context = context;
            _logger = logger;

        }
        
        public async Task<bool> CheckDatabaseHealthAsync()
        {
            try
            {
                // Simple query to check database connectivity
                await _context.Database.CanConnectAsync();

                // Verify we can read from essential tables
                var userCount = await _context.Users.CountAsync();
                var productCount = await _context.Products.CountAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                return false;
            }
        }

        // Bulk update product prices by percentage
        public async Task<int> UpdateProductPricesAsync(decimal percentage)
        {
            return await _context.Products
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(p => p.Price, p => p.Price * (1 + percentage / 100))
                     .SetProperty(p => p.UpdatedAt, p => DateTime.UtcNow));
        }

        // Bulk delete old orders
        public async Task<int> DeleteOldOrdersAsync(DateTime cutoffDate)
        {
            return await _context.Orders
                .Where(o => o.OrderedAt < cutoffDate)
                .ExecuteDeleteAsync();
        }

        // Then enhance methods with logging:
        public async Task<int> OptInAllUsersToNewsletterAsync()
        {
            _logger.LogInformation("Starting bulk opt-in for newsletter");

            var users = await _context.Users.ToListAsync();
            _logger.LogInformation("Found {UserCount} users to update", users.Count);

            foreach (var user in users)
            {
                user.Preferences.ReceiveNewsletter = true;
            }

            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("Updated {UpdatedCount} users for newsletter opt-in", result);

            return result;
        }

        // Add this method to BulkOperationService.cs
        public async Task<int> BatchUpdateProductInventoryAsync(Dictionary<int, int> productQuantityChanges)
        {
            // Using multiple operations in a single transaction is more efficient
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int totalUpdated = 0;

                // Execute multiple bulk operations in one transaction
                foreach (var batch in productQuantityChanges.Chunk(100))
                {
                    var productIds = batch.Select(x => x.Key).ToList();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();

                    foreach (var product in products)
                    {
                        if (productQuantityChanges.TryGetValue(product.Id, out int change))
                        {
                            product.Quantity += change;
                            product.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    totalUpdated += await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return totalUpdated;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Add this method to BulkOperationService.cs
        public async Task<List<Product>> GetProductsWithFullDetailsAsync(
            bool includeInventoryDetails = false,
            bool includeSalesHistory = false)
        {
            // Build query conditionally for better performance
            IQueryable<Product> query = _context.Products;

            if (includeInventoryDetails)
            {
                query = query.Include(p => p.ProductDetail);
            }

            if (includeSalesHistory)
            {
                query = query
                    .Include(p => p.Tags)
                    .AsSplitQuery(); // Split query for better performance with multiple includes
            }

            return await query.ToListAsync();
        }
        // Add this method to BulkOperationService.cs
        public async Task<bool> ApplyMigrationsAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                return false;
            }
        }
        // Add these methods to BulkOperationService.cs
        public async Task<(List<Product> Items, int TotalCount)> GetPaginatedProductsAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Products.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
