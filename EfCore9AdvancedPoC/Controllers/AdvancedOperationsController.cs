using EfCore9AdvancedPoCWithPostgres.Models;
using EfCore9AdvancedPoCWithPostgres.Repositories;
using EfCore9AdvancedPoCWithPostgres.Services;
using Microsoft.AspNetCore.Mvc;

namespace EfCore9AdvancedPoCWithPostgres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvancedOperationsController(
        BulkOperationService bulkService,
        IProductRepository productRepository) : ControllerBase
    {
        private readonly BulkOperationService _bulkService = bulkService;
        private readonly IProductRepository _productRepository = productRepository;

        [HttpGet("health-check")]
        public async Task<IActionResult> CheckHealth()
        {
            var isHealthy = await _bulkService.CheckDatabaseHealthAsync();
            return Ok(new { Status = isHealthy ? "Healthy" : "Unhealthy" });
        }

        [HttpGet("product-details")]
        public async Task<IActionResult> GetProductsWithDetails(
            [FromQuery] bool includeInventory = false,
            [FromQuery] bool includeSalesHistory = false)
        {
            var products = await _bulkService.GetProductsWithFullDetailsAsync(
                includeInventory, includeSalesHistory);

            return Ok(new
            {
                TotalCount = products.Count,
                Products = products
            });
        }

        [HttpGet("products/{page}")]
        public async Task<IActionResult> GetPaginatedProducts(int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bulkService.GetPaginatedProductsAsync(page, pageSize);

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = result.TotalCount,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                result.Items
            });
        }

        [HttpPost("batch-inventory-update")]
        public async Task<IActionResult> UpdateInventoryInBatch([FromBody] Dictionary<int, int> inventoryChanges)
        {
            if (inventoryChanges == null || !inventoryChanges.Any())
            {
                return BadRequest("No inventory changes provided");
            }

            var updatedCount = await _bulkService.BatchUpdateProductInventoryAsync(inventoryChanges);

            return Ok(new
            {
                UpdatedProducts = updatedCount,
                Message = $"Successfully updated {updatedCount} products"
            });
        }

        // Repository pattern demonstration
        [HttpGet("repository/products")]
        public async Task<IActionResult> GetProductsUsingRepository()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("repository/products/{id}")]
        public async Task<IActionResult> GetProductByIdUsingRepository(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("repository/products")]
        public async Task<IActionResult> CreateProductUsingRepository([FromBody] ProductCreateDto productDto)
        {
            if (productDto == null)
                return BadRequest();

            // Map DTO to entity
            var product = new Product
            {
                Name = productDto.Name,
                Quantity = productDto.Quantity,
                Price = productDto.Price,
                CreatedAt = productDto.CreatedAt,
                UpdatedAt = productDto.UpdatedAt,
                ValidFrom = productDto.ValidFrom,
                ValidTo = productDto.ValidTo,
                ProductDetail = new ProductDetail
                {
                    Description = productDto.ProductDetail?.Description,
                    Specifications = productDto.ProductDetail?.Specifications,
                    Manufacturer = productDto.ProductDetail?.Manufacturer,
                    ImageUrl = productDto.ProductDetail?.ImageUrl
                }
            };

            var result = await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetProductByIdUsingRepository), new { id = result.Id }, result);
        }

        [HttpPut("repository/products/{id}")]
        public async Task<IActionResult> UpdateProductUsingRepository(int id, [FromBody] ProductCreateDto productDto)
        {
            if (productDto == null)
                return BadRequest();

            if (!await _productRepository.ExistsAsync(id))
                return NotFound();

            // Get the existing product first
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            // Update the product properties from DTO
            existingProduct.Name = productDto.Name;
            existingProduct.Quantity = productDto.Quantity;
            existingProduct.Price = productDto.Price;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            // Update temporal fields if provided
            if (productDto.ValidFrom != default)
                existingProduct.ValidFrom = productDto.ValidFrom;
            if (productDto.ValidTo != default)
                existingProduct.ValidTo = productDto.ValidTo;

            // Update or create ProductDetail
            if (existingProduct.ProductDetail == null)
            {
                existingProduct.ProductDetail = new ProductDetail
                {
                    ProductId = id,
                    Description = productDto.ProductDetail?.Description,
                    Specifications = productDto.ProductDetail?.Specifications,
                    Manufacturer = productDto.ProductDetail?.Manufacturer,
                    ImageUrl = productDto.ProductDetail?.ImageUrl
                };
            }
            else
            {
                // Update existing product detail
                existingProduct.ProductDetail.Description = productDto.ProductDetail?.Description ?? existingProduct.ProductDetail.Description;
                existingProduct.ProductDetail.Specifications = productDto.ProductDetail?.Specifications ?? existingProduct.ProductDetail.Specifications;
                existingProduct.ProductDetail.Manufacturer = productDto.ProductDetail?.Manufacturer ?? existingProduct.ProductDetail.Manufacturer;
                existingProduct.ProductDetail.ImageUrl = productDto.ProductDetail?.ImageUrl ?? existingProduct.ProductDetail.ImageUrl;
            }

            // Update the reference back to the product to resolve circular reference
            existingProduct.ProductDetail.Product = existingProduct;

            await _productRepository.UpdateAsync(existingProduct);
            return NoContent();
        }

        [HttpDelete("repository/products/{id}")]
        public async Task<IActionResult> DeleteProductUsingRepository(int id)
        {
            if (!await _productRepository.ExistsAsync(id))
                return NotFound();

            await _productRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
