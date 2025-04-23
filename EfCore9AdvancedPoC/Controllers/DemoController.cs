using EfCore9AdvancedPoCWithPostgres.Models;
using EfCore9AdvancedPoCWithPostgres.Models.Inheritance;
using EfCore9AdvancedPoCWithPostgres.Models.Json;
using EfCore9AdvancedPoCWithPostgres.Models.Relationships;
using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EfCore9AdvancedPoC.Models.Inheritance;

namespace EfCore9AdvancedPoCWithPostgres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BulkOperationService _bulkService;

        public DemoController(AppDbContext context, BulkOperationService bulkService)
        {
            _context = context;
            _bulkService = bulkService;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                // Clear any existing data using LINQ instead of raw SQL
                var productDetails = await _context.Set<ProductDetail>().ToListAsync();
                _context.Set<ProductDetail>().RemoveRange(productDetails);

                var orders = await _context.Orders.ToListAsync();
                _context.Orders.RemoveRange(orders);

                var products = await _context.Products.ToListAsync();
                _context.Products.RemoveRange(products);

                var users = await _context.Users.ToListAsync();
                _context.Users.RemoveRange(users);

                var customerEntities = await _context.CustomerEntities.ToListAsync();
                _context.CustomerEntities.RemoveRange(customerEntities);

                var employeeEntities = await _context.EmployeeEntities.ToListAsync();
                _context.EmployeeEntities.RemoveRange(employeeEntities);

                var baseEntities = await _context.BaseEntities.ToListAsync();
                _context.BaseEntities.RemoveRange(baseEntities);

                // Remove any existing employees
                var employees = await _context.Employees.ToListAsync();
                _context.Employees.RemoveRange(employees);

                // Remove any existing tags and product tags
                var productTags = await _context.ProductTags.ToListAsync();
                _context.ProductTags.RemoveRange(productTags);

                var tags = await _context.Tags.ToListAsync();
                _context.Tags.RemoveRange(tags);

                await _context.SaveChangesAsync();

                // Add users
                var user1 = new User
                {
                    FullName = "John Smith",
                    Preferences = new Models.Owned.UserPreferences
                    {
                        Theme = "dark",
                        ReceiveNewsletter = true
                    }
                };

                var user2 = new User
                {
                    FullName = "Jane Doe",
                    Preferences = new Models.Owned.UserPreferences
                    {
                        Theme = "light",
                        ReceiveNewsletter = false
                    }
                };

                _context.Users.AddRange(user1, user2);
                await _context.SaveChangesAsync();

                var now = DateTime.UtcNow; // Use a consistent timestamp

                var newProducts = new List<Product>
{
    new Product
    {
        Name = "Laptop",
        Quantity = 10,
        Price = 1200.00m,
        CreatedAt = now,
        UpdatedAt = now,
        ValidFrom = now,              // Ensure this is set
        ValidTo = DateTime.MaxValue   // Standard value for temporal "to" date
    },
    new Product
    {
        Name = "Phone",
        Quantity = 20,
        Price = 800.00m,
        CreatedAt = now,
        UpdatedAt = now,
        ValidFrom = now,              // Ensure this is set
        ValidTo = DateTime.MaxValue   // Standard value for temporal "to" date
    },
    new Product
    {
        Name = "Headphones",
        Quantity = 30,
        Price = 150.00m,
        CreatedAt = now,
        UpdatedAt = now,
        ValidFrom = now,              // Ensure this is set
        ValidTo = DateTime.MaxValue   // Standard value for temporal "to" date
    }
};// Ensure temporal fields are explicitly set before saving
               

                _context.Products.AddRange(newProducts);
                foreach (var product in newProducts)
                {
                    // Make sure these fields are not null
                    if (product.ValidFrom == default)
                        product.ValidFrom = now;

                    if (product.ValidTo == default)
                        product.ValidTo = DateTime.MaxValue;
                }

                await _context.SaveChangesAsync();

                // Add product details
                foreach (var product in newProducts)
                {

                    var detail = new ProductDetail
                    {
                        ProductId = product.Id,
                        Description = $"{product.Name} description",
                        Specifications = product.Name == "Laptop" ? "16GB RAM, 512GB SSD" :
                                       product.Name == "Phone" ? "6GB RAM, 128GB Storage" : "Wireless, 20hr battery",
                        Manufacturer = product.Name == "Headphones" ? "AudioTech" : "TechCorp",
                        ImageUrl = $"/images/{product.Name.ToLower()}.jpg"
                    };

                    _context.Set<ProductDetail>().Add(detail);
                }

                await _context.SaveChangesAsync();

                // Add tags
                var tagsList = new List<Tag>
{
    new Tag { Name = "electronics" },
    new Tag { Name = "computer" },
    new Tag { Name = "mobile" },
    new Tag { Name = "audio" }
};

                _context.Tags.AddRange(tagsList);
                await _context.SaveChangesAsync();

                // Add product tags
                var productTagsList = new List<ProductTag>
{
    new ProductTag
    {
        ProductId = newProducts[0].Id, // Laptop
        TagId = tagsList[0].Id, // electronics
        AssignedOn = DateTime.UtcNow,
        AssignedBy = "System"
    },
    new ProductTag
    {
        ProductId = newProducts[0].Id, // Laptop
        TagId = tagsList[1].Id, // computer
        AssignedOn = DateTime.UtcNow,
        AssignedBy = "System"
    },
    new ProductTag
    {
        ProductId = newProducts[1].Id, // Phone
        TagId = tagsList[0].Id, // electronics
        AssignedOn = DateTime.UtcNow,
        AssignedBy = "System"
    },
    new ProductTag
    {
        ProductId = newProducts[1].Id, // Phone
        TagId = tagsList[2].Id, // mobile
        AssignedOn = DateTime.UtcNow,
        AssignedBy = "System"
    },
    new ProductTag
    {
        ProductId = newProducts[2].Id, // Headphones
        TagId = tagsList[0].Id, // electronics
        AssignedOn = DateTime.UtcNow,
        AssignedBy = "System"
    },
    new ProductTag
    {
        ProductId = newProducts[2].Id, // Headphones
        TagId = tagsList[3].Id, // audio
        AssignedOn = DateTime.UtcNow,
        AssignedBy = "System"
    }
};

                _context.ProductTags.AddRange(productTagsList);
                await _context.SaveChangesAsync();

                // Add orders with JSON data
                var order1 = new Order
                {
                    UserId = user1.Id,
                    ProductId = newProducts[0].Id,
                    OrderedAt = DateTime.UtcNow,
                    Details = new OrderDetails
                    {
                        ProductName = newProducts[0].Name,
                        Quantity = 1,
                        Price = newProducts[0].Price,
                        Tags = new[] { "electronics", "computer" },
                        Metadata = new Dictionary<string, string>
                        {
                            { "warranty", "2 years" },
                            { "processor", "i7" }
                        }
                    },
                    ShippingAddress = new Models.Owned.ShippingAddress
                    {
                        Line1 = "123 Main St",
                        City = "New York",
                        PostalCode = "10001"
                    }
                };

                var order2 = new Order
                {
                    UserId = user2.Id,
                    ProductId = newProducts[1].Id,
                    OrderedAt = DateTime.UtcNow,
                    Details = new OrderDetails
                    {
                        ProductName = newProducts[1].Name,
                        Quantity = 1,
                        Price = newProducts[1].Price,
                        Tags = new[] { "electronics", "mobile" },
                        Metadata = new Dictionary<string, string>
                        {
                            { "warranty", "1 year" },
                            { "color", "black" }
                        }
                    },
                    ShippingAddress = new Models.Owned.ShippingAddress
                    {
                        Line1 = "456 Oak Avenue",
                        City = "Chicago",
                        PostalCode = "60007"
                    }
                };

                _context.Orders.AddRange(order1, order2);
                await _context.SaveChangesAsync();


                // Create employee hierarchy for self-referencing relationship demo
                var cto = new Employee { Name = "Alice Johnson", Position = "CTO", Salary = 180000 };
                await _context.Employees.AddAsync(cto);
                await _context.SaveChangesAsync();

                var managers = new List<Employee>
                {
                    new Employee { Name = "Bob Smith", Position = "Engineering Manager", Salary = 140000, ManagerId = cto.Id },
                    new Employee { Name = "Carol White", Position = "Product Manager", Salary = 135000, ManagerId = cto.Id }
                };

                await _context.Employees.AddRangeAsync(managers);
                await _context.SaveChangesAsync();

                var engineers = new List<Employee>
                {
                    new Employee { Name = "Dave Brown", Position = "Senior Engineer", Salary = 120000, ManagerId = managers[0].Id },
                    new Employee { Name = "Eve Black", Position = "Engineer", Salary = 95000, ManagerId = managers[0].Id },
                    new Employee { Name = "Frank Green", Position = "Product Designer", Salary = 110000, ManagerId = managers[1].Id }
                };

                await _context.Employees.AddRangeAsync(engineers);
                await _context.SaveChangesAsync();

                // Add TPT inheritance entities
                var employee = new EmployeeEntity
                {
                    Name = "Mark Wilson",
                    Department = "Engineering",
                    Position = "Software Developer",
                    Salary = 95000,
                    CreatedAt = DateTime.UtcNow
                };

                _context.EmployeeEntities.Add(employee);
                await _context.SaveChangesAsync();

                // Add this code right after adding the EmployeeEntity in your SeedData method
                var customer = new CustomerEntity
                {
                    Name = "Sarah Johnson",
                    Email = "sarah@example.com",    // This will be automatically encrypted
                    PhoneNumber = "555-123-4567",
                    CreatedAt = DateTime.UtcNow
                };

                _context.CustomerEntities.Add(customer);
                await _context.SaveChangesAsync();

                return Ok("Database seeded successfully with all entity types");
            }
            catch (Exception ex)
            {
                // Return a detailed error response for debugging
                var fullError = $"Error seeding database: {ex.Message}";
                if (ex.InnerException != null)
                    fullError += $"\nInner exception: {ex.InnerException.Message}";

                return StatusCode(500, fullError);
            }
        }

        [HttpGet("json-query")]
        public async Task<IActionResult> QueryJsonData()
        {
            // Query using JSON path expressions
            var laptopOrders = await _context.Orders
                .Where(o => o.Details.ProductName == "Laptop")
                .ToListAsync();

            // Extract orders with a specific tag using EF.Functions.JsonContains
            var electronicsOrders = await _context.Orders
                .Where(o => EF.Functions.JsonContains(
                    EF.Property<string>(o, "Details"),
                    @"{""Tags"": [""electronics""]}"
                ))
                .ToListAsync();

            return Ok(new
            {
                LaptopOrders = laptopOrders,
                ElectronicsOrders = electronicsOrders
            });
        }

        [HttpPost("bulk-operations")]
        public async Task<IActionResult> RunBulkOperations()
        {
            // 1. Bulk update all product prices by 10%
            var updatedProductsCount = await _bulkService.UpdateProductPricesAsync(10);

            // 2. Bulk delete old orders (none in this demo)
            var deletedOrdersCount = await _bulkService.DeleteOldOrdersAsync(DateTime.UtcNow.AddYears(-1));

            // 3. Opt-in all users to newsletter
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
            // First modify a product to demonstrate temporal table
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            // Record original price
            var originalPrice = product.Price;

            // Update price to demonstrate temporal change
            product.Price *= 1.15m; // 15% increase
            await _context.SaveChangesAsync();

            // Query just the current record (safer approach)
            var currentProduct = await _context.Products
                .Where(p => p.Id == productId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Quantity,
                    ValidFrom = EF.Property<DateTime>(p, "ValidFrom"),
                    ValidTo = EF.Property<DateTime>(p, "ValidTo")
                })
                .ToListAsync();

            return Ok(new
            {
                OriginalPrice = originalPrice,
                CurrentPrice = product.Price,
                PriceHistory = currentProduct
            });
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            // Retrieve all audit logs
            var logs = await _context.AuditLogs.ToListAsync();
            return Ok(logs);
        }

        [HttpGet("tpt-inheritance")]
        public async Task<IActionResult> GetInheritanceData()
        {
            // Query both employee and customer entities
            var employees = await _context.EmployeeEntities.ToListAsync();
            var customers = await _context.CustomerEntities.ToListAsync();

            // Also show polymorphic query
            var allEntities = await _context.BaseEntities
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();

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
            // Get all visible users
            var visibleUsers = await _context.Users.ToListAsync();

            // Soft delete the first user
            var user = visibleUsers.FirstOrDefault();
            if (user != null)
            {
                user.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            // Get visible users after soft delete
            var remainingUsers = await _context.Users.ToListAsync();

            // Get ALL users including deleted (bypassing global query filter)
            var allUsers = await _context.Users
                .IgnoreQueryFilters()
                .ToListAsync();

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
            // First, find an existing user ID to demonstrate with
            var userId = await _context.Users
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0) // No users found
            {
                return NotFound("No users found. Please seed the database first.");
            }

            // Create a compiled query for better performance with frequently used queries
            var getUserByIdQuery = EF.CompileQuery(
                (AppDbContext context, int id) =>
                    context.Users
                        .IgnoreQueryFilters() // Ignore the soft delete filter
                        .Include(u => u.Orders)
                        .FirstOrDefault(u => u.Id == id));

            var user = getUserByIdQuery(_context, userId);

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
            // Split query helps with complex includes to avoid cartesian explosion
            var usersWithOrders = await _context.Users
                .AsSplitQuery()  // This makes separate SQL queries instead of one big join
                .Include(u => u.Orders)
                    .ThenInclude(o => o.Product)
                        .ThenInclude(p => p.ProductDetail)
                .ToListAsync();

            return Ok(new
            {
                Feature = "Split Query",
                Description = "Separate queries for related data to avoid cartesian explosion",
                UsersCount = usersWithOrders.Count,
                TotalOrders = usersWithOrders.Sum(u => u.Orders.Count)
            });
        }

        [HttpGet("shadow-properties")]
        public async Task<IActionResult> DemonstrateShadowProperties()
        {
            // Shadow properties are not defined in entity class but exist in the database

            // Set shadow property value
            var product = await _context.Products.FirstAsync();
            _context.Entry(product).Property("LastViewedAt").CurrentValue = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Query using shadow property
            var recentlyViewedProducts = await _context.Products
                .Where(p => EF.Property<DateTime>(p, "LastViewedAt") > DateTime.UtcNow.AddMinutes(-30))
                .ToListAsync();

            return Ok(new
            {
                Feature = "Shadow Properties",
                Description = "Properties tracked by EF Core but not defined in entity class",
                ProductId = product.Id,
                LastViewedAt = _context.Entry(product).Property("LastViewedAt").CurrentValue
            });
        }

        [HttpGet("change-tracking")]
        public async Task<IActionResult> DemonstrateChangeTracking()
        {
            // Get current tracking behavior
            var trackingBehavior = _context.ChangeTracker.QueryTrackingBehavior;

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var unTrackedProducts = await _context.Products.ToListAsync();

            // Reset to original tracking behavior
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            // Use AsNoTracking to avoid tracking conflicts
            var trackedProducts = await _context.Products.AsNoTracking().ToListAsync();

            // Manually attach and modify an entity
            var product = unTrackedProducts.First();
            _context.Attach(product);
            product.Price *= 1.05m;  // 5% increase

            // Check entity states
            var entries = _context.ChangeTracker.Entries()
                .Select(e => new
                {
                    Entity = e.Entity.GetType().Name,
                    Id = e.Property("Id").CurrentValue,
                    State = e.State.ToString()
                })
                .ToList();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Feature = "Change Tracking Customization",
                Description = "Control how EF Core tracks entity changes",
                TrackingBehavior = trackingBehavior.ToString(),
                TrackedEntities = entries,
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
            // Query existing managers with LINQ instead of raw SQL
            var existingCTO = await _context.Employees
                .FirstOrDefaultAsync(e => e.Position == "CTO");

            if (existingCTO == null)
            {
                // Create a simple organizational hierarchy
                var cto = new Employee { Name = "Alice Johnson", Position = "CTO", Salary = 180000 };
                await _context.Employees.AddAsync(cto);
                await _context.SaveChangesAsync();

                var managers = new List<Employee>
                {
                    new Employee { Name = "Bob Smith", Position = "Engineering Manager", Salary = 140000, ManagerId = cto.Id },
                    new Employee { Name = "Carol White", Position = "Product Manager", Salary = 135000, ManagerId = cto.Id }
                };

                await _context.Employees.AddRangeAsync(managers);
                await _context.SaveChangesAsync();

                var engineers = new List<Employee>
                {
                    new Employee { Name = "Dave Brown", Position = "Senior Engineer", Salary = 120000, ManagerId = managers[0].Id },
                    new Employee { Name = "Eve Black", Position = "Engineer", Salary = 95000, ManagerId = managers[0].Id },
                    new Employee { Name = "Frank Green", Position = "Product Designer", Salary = 110000, ManagerId = managers[1].Id }
                };

                await _context.Employees.AddRangeAsync(engineers);
                await _context.SaveChangesAsync();
            }

            // Query org chart with hierarchy
            var orgChart = await _context.Employees
                .Include(e => e.DirectReports)
                .Where(e => e.Manager == null) // Get top-level managers
                .ToListAsync();

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

        // Add a table splitting example with Product/ProductDetail
        [HttpGet("table-splitting")]
        public async Task<IActionResult> DemonstrateTableSplitting()
        {
            var products = await _context.Products
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Quantity,
                    p.ProductDetail.Description,
                    p.ProductDetail.Manufacturer,
                    p.ProductDetail.Specifications
                })
                .ToListAsync();

            return Ok(new
            {
                Feature = "Table Splitting",
                Description = "Multiple entity types mapped to same table with shared primary key",
                Products = products
            });
        }
    }
}