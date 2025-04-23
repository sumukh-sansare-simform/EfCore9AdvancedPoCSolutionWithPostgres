using EfCore9AdvancedPoC.Models;
using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;

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
            
        public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids) => 
            await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();

        // Add this to your ProductRepository.cs
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

        // Also update the UpdateAsync method to handle the same issue
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

            // Handle ProductTags if needed
            if (product.ProductTags != null && product.ProductTags.Any())
            {
                // This would need more complex logic to properly handle adding/removing tags
                // For now, we're not updating tags in this example
            }

            return await _context.SaveChangesAsync();
        }


        public async Task<int> DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product == null) return 0;
            
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }
        
        public async Task<bool> ExistsAsync(int id) => 
            await _context.Products.AnyAsync(p => p.Id == id);
    }
}
