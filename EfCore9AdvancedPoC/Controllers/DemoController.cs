using EfCore9AdvancedPoCWithPostgres.Repositories;
using EfCore9AdvancedPoCWithPostgres.Services;
using Microsoft.AspNetCore.Mvc;

namespace EfCore9AdvancedPoCWithPostgres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IBaseEntityRepository _baseEntityRepository;
        private readonly BulkOperationService _bulkService;
        private readonly DataSeedingService _dataSeedingService;

        public DemoController(
            IUserRepository userRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository, 
            IEmployeeRepository employeeRepository,
            IAuditLogRepository auditLogRepository,
            IBaseEntityRepository baseEntityRepository,
            BulkOperationService bulkService,
            DataSeedingService dataSeedingService)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _employeeRepository = employeeRepository;
            _auditLogRepository = auditLogRepository;
            _baseEntityRepository = baseEntityRepository;
            _bulkService = bulkService;
            _dataSeedingService = dataSeedingService;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                var success = await _dataSeedingService.SeedDatabaseAsync();
                if (success)
                {
                    return Ok("Database seeded successfully with all entity types");
                }
                else
                {
                    return StatusCode(500, "Error seeding database");
                }
            }
            catch (Exception ex)
            {
                var fullError = $"Error seeding database: {ex.Message}";
                if (ex.InnerException != null)
                    fullError += $"\nInner exception: {ex.InnerException.Message}";

                return StatusCode(500, fullError);
            }
        }

        [HttpGet("json-query")]
        public async Task<IActionResult> QueryJsonData()
        {
            // Use specialized repository methods
            var laptopOrders = await _orderRepository.GetOrdersByProductNameAsync("Laptop");
            var electronicsOrders = await _orderRepository.GetOrdersWithTagAsync("electronics");

            return Ok(new
            {
                LaptopOrders = laptopOrders,
                ElectronicsOrders = electronicsOrders
            });
        }

        [HttpPost("bulk-operations")]
        public async Task<IActionResult> RunBulkOperations()
        {
            // Using service methods
            var updatedProductsCount = await _bulkService.UpdateProductPricesAsync(10);
            var deletedOrdersCount = await _bulkService.DeleteOldOrdersAsync(DateTime.UtcNow.AddYears(-1));
            var updatedUsersCount = await _bulkService.OptInAllUsersToNewsletterAsync();

            return Ok(new
            {
                UpdatedProducts = updatedProductsCount,
                DeletedOrders = deletedOrdersCount,
                UpdatedUsers = updatedUsersCount
            });
        }

        [HttpGet("temporal-query/{productId}")]
        public async Task<IActionResult> QueryTemporalData(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            // Record original price
            var originalPrice = product.Price;

            // Update price to demonstrate temporal change
            product.Price *= 1.15m; // 15% increase
            await _productRepository.UpdateAsync(product);

            // For a complete implementation, you would have a special repository method
            // to query temporal data with period columns
            var updatedProduct = await _productRepository.GetByIdAsync(productId);

            return Ok(new
            {
                OriginalPrice = originalPrice,
                CurrentPrice = updatedProduct.Price
            });
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _auditLogRepository.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("tpt-inheritance")]
        public async Task<IActionResult> GetInheritanceData()
        {
            var employees = await _baseEntityRepository.GetAllEmployeeEntitiesAsync();
            var customers = await _baseEntityRepository.GetAllCustomerEntitiesAsync();
            var allEntities = await _baseEntityRepository.GetAllBaseEntitiesAsync();

            return Ok(new
            {
                Employees = employees,
                Customers = customers,
                AllEntities = allEntities.Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.CreatedAt,
                    EntityType = e.GetType().Name
                })
            });
        }

        [HttpGet("soft-delete")]
        public async Task<IActionResult> DemonstrateSoftDelete()
        {
            var visibleUsers = await _userRepository.GetAllAsync();

            // Soft delete the first user
            var user = visibleUsers.FirstOrDefault();
            if (user != null)
            {
                await _userRepository.SoftDeleteAsync(user.Id);
            }

            // Get visible users after soft delete
            var remainingUsers = await _userRepository.GetAllAsync();

            // Get ALL users including deleted
            var allUsers = await _userRepository.GetAllIncludingDeletedAsync();

            return Ok(new
            {
                OriginalCount = visibleUsers.Count,
                AfterSoftDeleteCount = remainingUsers.Count,
                AllUsersIncludingDeleted = allUsers.Count,
                DeletedUser = user
            });
        }

        [HttpGet("compiled-query")]
        public async Task<IActionResult> DemonstrateCompiledQuery()
        {
            // Find an existing user ID
            var users = await _userRepository.GetAllAsync();
            var userId = users.FirstOrDefault()?.Id ?? 0;

            if (userId == 0)
            {
                return NotFound("No users found. Please seed the database first.");
            }

            // Get user with orders using repository method
            var user = await _userRepository.GetWithOrdersAsync(userId);

            return Ok(new
            {
                Feature = "Compiled Query",
                Description = "Precompiled LINQ query for better performance",
                UserId = userId,
                User = user
            });
        }

        [HttpGet("split-query")]
        public async Task<IActionResult> DemonstrateSplitQuery()
        {
            // For a full implementation, you would add a repository method
            // specifically for retrieving users with orders and related data
            var users = await _userRepository.GetAllAsync();

            return Ok(new
            {
                Feature = "Split Query",
                Description = "Separate queries for related data to avoid cartesian explosion",
                UsersCount = users.Count
            });
        }

        [HttpGet("shadow-properties")]
        public async Task<IActionResult> DemonstrateShadowProperties()
        {
            // For a complete implementation, you would add repository methods
            // to work with shadow properties
            var products = await _productRepository.GetAllAsync();
            var product = products.FirstOrDefault();

            if (product == null)
            {
                return NotFound("No products found. Please seed the database first.");
            }

            // Update the last viewed time
            await _productRepository.SetLastViewedAtAsync(product.Id);

            // Get recently viewed products
            var recentlyViewedProducts = await _productRepository.GetRecentlyViewedProductsAsync(TimeSpan.FromMinutes(30));

            return Ok(new
            {
                Feature = "Shadow Properties",
                Description = "Properties tracked by EF Core but not defined in entity class",
                ProductId = product.Id,
                RecentlyViewedCount = recentlyViewedProducts.Count
            });
        }

        [HttpGet("change-tracking")]
        public async Task<IActionResult> DemonstrateChangeTracking()
        {
            // For a complete implementation, you would add repository methods
            // to demonstrate change tracking
            var products = await _productRepository.GetAllAsync();
            var product = products.FirstOrDefault();

            if (product == null)
            {
                return NotFound("No products found. Please seed the database first.");
            }

            // Update product price
            product.Price *= 1.05m; // 5% increase
            await _productRepository.UpdateAsync(product);

            return Ok(new
            {
                Feature = "Change Tracking Customization",
                Description = "Control how EF Core tracks entity changes",
                ModifiedProduct = new
                {
                    product.Id,
                    product.Name,
                    product.Price
                }
            });
        }

        [HttpGet("self-referencing")]
        public async Task<IActionResult> DemonstrateSelfReferencing()
        {
            // Use repository method to get organizational hierarchy
            var orgChart = await _employeeRepository.GetOrganizationalHierarchyAsync();

            if (!orgChart.Any())
            {
                // Create a simple organizational hierarchy via DataSeedingService
                await _dataSeedingService.SeedDatabaseAsync();
                orgChart = await _employeeRepository.GetOrganizationalHierarchyAsync();
            }

            return Ok(new
            {
                Feature = "Self-Referencing Relationships",
                Description = "Entities that reference their own type (like hierarchies)",
                OrganizationalChart = orgChart.Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Position,
                    e.Salary,
                    Reports = e.DirectReports.Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Position,
                        r.Salary,
                        Reports = r.DirectReports.Select(sr => new
                        {
                            sr.Id,
                            sr.Name,
                            sr.Position,
                            sr.Salary
                        })
                    })
                })
            });
        }

        [HttpGet("table-splitting")]
        public async Task<IActionResult> DemonstrateTableSplitting()
        {
            var products = await _productRepository.GetProductsWithDetailsAsync();

            return Ok(new
            {
                Feature = "Table Splitting",
                Description = "Multiple entity types mapped to same table with shared primary key",
                Products = products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Quantity,
                    p.ProductDetail?.Description,
                    p.ProductDetail?.Manufacturer,
                    p.ProductDetail?.Specifications
                })
            });
        }
    }
}
