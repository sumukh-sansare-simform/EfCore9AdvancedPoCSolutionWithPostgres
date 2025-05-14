using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Product>> GetAllAsync() => 
            await _context.Products.ToListAsync();
            
        public async Task<Product> GetByIdAsync(int id) => 
            await _context.Products.FindAsync(id);
            
        public async Task<List<Product>> FindAsync(Expression<Func<Product, bool>> predicate) => 
            await _context.Products.Where(predicate).ToListAsync();

        public async Task<List<Product>> GetProductsWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductDetail)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsWithTagsAsync()
        {
            return await _context.Products
                .Include(p => p.Tags)
                .ToListAsync();
        }

        public async Task<List<Product>> GetRecentlyViewedProductsAsync(TimeSpan timeSpan)
        {
            // Calculate the cut-off time on the C# side
            var cutoffTime = DateTime.UtcNow.AddTicks(-timeSpan.Ticks);

            // Then use the calculated time in the query
            return await _context.Products
                .Where(p => EF.Property<DateTime>(p, "LastViewedAt") > cutoffTime)
                .ToListAsync();
        }

        public async Task SetLastViewedAtAsync(int productId)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                _context.Entry(product).Property("LastViewedAt").CurrentValue = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Product> AddAsync(Product product)
        {
            // Fix circular reference between Product and ProductDetail
            if (product.ProductDetail != null)
            {
                product.ProductDetail.Product = product; // Set the back-reference
            }

            // Fix circular references in ProductTags collection
            if (product.ProductTags != null)
            {
                foreach (var pt in product.ProductTags)
                {
                    pt.Product = product; // Set the back-reference
                }
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<int> UpdateAsync(Product product)
        {
            // First, fetch the existing product including its details to properly track changes
            var existingProduct = await _context.Products
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {product.Id} not found");

            // Update product properties
            _context.Entry(existingProduct).CurrentValues.SetValues(product);

            // Update product detail - handle it separately
            if (product.ProductDetail != null)
            {
                if (existingProduct.ProductDetail == null)
                {
                    // If there was no product detail before, create a new one
                    existingProduct.ProductDetail = new ProductDetail
                    {
                        ProductId = product.Id,
                        Description = product.ProductDetail.Description,
                        Specifications = product.ProductDetail.Specifications,
                        Manufacturer = product.ProductDetail.Manufacturer,
                        ImageUrl = product.ProductDetail.ImageUrl
                    };
                }
                else
                {
                    // Update existing product detail
                    _context.Entry(existingProduct.ProductDetail).CurrentValues.SetValues(product.ProductDetail);
                }

                // Ensure the circular reference is set correctly
                existingProduct.ProductDetail.Product = existingProduct;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
            return await _context.SaveChangesAsync();
        }
        
        public async Task<int> DeleteByIdAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product == null) return 0;
            
            return await DeleteAsync(product);
        }
        
        public async Task<bool> ExistsAsync(int id) => 
            await _context.Products.AnyAsync(p => p.Id == id);

        public async Task<int> CountAsync() =>
            await _context.Products.CountAsync();
    }
}
