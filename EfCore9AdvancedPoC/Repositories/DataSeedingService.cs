using EfCore9AdvancedPoC.Models.Inheritance;
using EfCore9AdvancedPoCWithPostgres.Models;
using EfCore9AdvancedPoCWithPostgres.Models.Inheritance;
using EfCore9AdvancedPoCWithPostgres.Models.Json;
using EfCore9AdvancedPoCWithPostgres.Models.Relationships;
using EfCore9AdvancedPoCWithPostgres.Repositories;

namespace EfCore9AdvancedPoCWithPostgres.Services
{
    public class DataSeedingService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBaseEntityRepository _baseEntityRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<DataSeedingService> _logger;

        public DataSeedingService(
            IUserRepository userRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IEmployeeRepository employeeRepository,
            IBaseEntityRepository baseEntityRepository,
            ITagRepository tagRepository,
            ILogger<DataSeedingService> logger)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _employeeRepository = employeeRepository;
            _baseEntityRepository = baseEntityRepository;
            _tagRepository = tagRepository;
            _logger = logger;
        }

        public async Task<bool> SeedDatabaseAsync()
        {
            try
            {
                // Clear existing data
                await ClearExistingDataAsync();

                // Add users
                var users = await SeedUsersAsync();
                
                // Add products
                var products = await SeedProductsAsync();
                
                // Add tags
                var tags = await SeedTagsAsync();
                
                // Add product tags
                await SeedProductTagsAsync(products, tags);
                
                // Add orders
                await SeedOrdersAsync(users, products);
                
                // Add employees
                await SeedEmployeesAsync();
                
                // Add TPT inheritance entities
                await SeedInheritanceEntitiesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding database");
                return false;
            }
        }

        private async Task ClearExistingDataAsync()
        {
            // Clear existing data in reverse order of dependencies
            var products = await _productRepository.GetAllAsync();
            foreach (var product in products)
            {
                await _productRepository.DeleteAsync(product);
            }

            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
            {
                await _userRepository.DeleteAsync(user);
            }

            var employees = await _employeeRepository.GetAllAsync();
            foreach (var employee in employees)
            {
                await _employeeRepository.DeleteAsync(employee);
            }

            // Other entity cleanup as needed
        }

        private async Task<List<User>> SeedUsersAsync()
        {
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

            await _userRepository.AddAsync(user1);
            await _userRepository.AddAsync(user2);

            return new List<User> { user1, user2 };
        }

        private async Task<List<Product>> SeedProductsAsync()
        {
            var now = DateTime.UtcNow;

            var newProducts = new List<Product>
            {
                new Product
                {
                    Name = "Laptop",
                    Quantity = 10,
                    Price = 1200.00m,
                    CreatedAt = now,
                    UpdatedAt = now,
                    ValidFrom = now,
                    ValidTo = DateTime.MaxValue,
                    ProductDetail = new ProductDetail
                    {
                        Description = "Laptop description",
                        Specifications = "16GB RAM, 512GB SSD",
                        Manufacturer = "TechCorp",
                        ImageUrl = "/images/laptop.jpg"
                    }
                },
                new Product
                {
                    Name = "Phone",
                    Quantity = 20,
                    Price = 800.00m,
                    CreatedAt = now,
                    UpdatedAt = now,
                    ValidFrom = now,
                    ValidTo = DateTime.MaxValue,
                    ProductDetail = new ProductDetail
                    {
                        Description = "Phone description",
                        Specifications = "6GB RAM, 128GB Storage",
                        Manufacturer = "TechCorp",
                        ImageUrl = "/images/phone.jpg"
                    }
                },
                new Product
                {
                    Name = "Headphones",
                    Quantity = 30,
                    Price = 150.00m,
                    CreatedAt = now,
                    UpdatedAt = now,
                    ValidFrom = now,
                    ValidTo = DateTime.MaxValue,
                    ProductDetail = new ProductDetail
                    {
                        Description = "Headphones description",
                        Specifications = "Wireless, 20hr battery",
                        Manufacturer = "AudioTech",
                        ImageUrl = "/images/headphones.jpg"
                    }
                }
            };

            foreach (var product in newProducts)
            {
                product.ProductDetail.Product = product; // Set circular reference
                await _productRepository.AddAsync(product);
            }

            return newProducts;
        }

        private async Task<List<Tag>> SeedTagsAsync()
        {
            var tags = new List<Tag>
            {
                new Tag { Name = "electronics" },
                new Tag { Name = "computer" },
                new Tag { Name = "mobile" },
                new Tag { Name = "audio" }
            };

            foreach (var tag in tags)
            {
                await _tagRepository.AddAsync(tag);
            }

            return tags;
        }

        private async Task SeedProductTagsAsync(List<Product> products, List<Tag> tags)
        {
            // Laptop tags
            await _tagRepository.AddTagToProductAsync(products[0].Id, tags[0].Id, "System");
            await _tagRepository.AddTagToProductAsync(products[0].Id, tags[1].Id, "System");

            // Phone tags
            await _tagRepository.AddTagToProductAsync(products[1].Id, tags[0].Id, "System");
            await _tagRepository.AddTagToProductAsync(products[1].Id, tags[2].Id, "System");

            // Headphone tags
            await _tagRepository.AddTagToProductAsync(products[2].Id, tags[0].Id, "System");
            await _tagRepository.AddTagToProductAsync(products[2].Id, tags[3].Id, "System");
        }

        private async Task SeedOrdersAsync(List<User> users, List<Product> products)
        {
            var order1 = new Order
            {
                UserId = users[0].Id,
                ProductId = products[0].Id,
                OrderedAt = DateTime.UtcNow,
                Details = new OrderDetails
                {
                    ProductName = products[0].Name,
                    Quantity = 1,
                    Price = products[0].Price,
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
                UserId = users[1].Id,
                ProductId = products[1].Id,
                OrderedAt = DateTime.UtcNow,
                Details = new OrderDetails
                {
                    ProductName = products[1].Name,
                    Quantity = 1,
                    Price = products[1].Price,
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

            await _orderRepository.AddAsync(order1);
            await _orderRepository.AddAsync(order2);
        }

        private async Task SeedEmployeesAsync()
        {
            // Create CTO
            var cto = new Employee { Name = "Alice Johnson", Position = "CTO", Salary = 180000 };
            await _employeeRepository.AddAsync(cto);

            // Create managers reporting to CTO
            var managers = new List<Employee>
            {
                new Employee { Name = "Bob Smith", Position = "Engineering Manager", Salary = 140000, ManagerId = cto.Id },
                new Employee { Name = "Carol White", Position = "Product Manager", Salary = 135000, ManagerId = cto.Id }
            };

            foreach (var manager in managers)
            {
                await _employeeRepository.AddAsync(manager);
            }

            // Create engineers reporting to managers
            var engineers = new List<Employee>
            {
                new Employee { Name = "Dave Brown", Position = "Senior Engineer", Salary = 120000, ManagerId = managers[0].Id },
                new Employee { Name = "Eve Black", Position = "Engineer", Salary = 95000, ManagerId = managers[0].Id },
                new Employee { Name = "Frank Green", Position = "Product Designer", Salary = 110000, ManagerId = managers[1].Id }
            };

            foreach (var engineer in engineers)
            {
                await _employeeRepository.AddAsync(engineer);
            }
        }

        private async Task SeedInheritanceEntitiesAsync()
        {
            var employee = new EmployeeEntity
            {
                Name = "Mark Wilson",
                Department = "Engineering",
                Position = "Software Developer",
                Salary = 95000,
                CreatedAt = DateTime.UtcNow
            };

            await _baseEntityRepository.AddEmployeeEntityAsync(employee);

            var customer = new CustomerEntity
            {
                Name = "Sarah Johnson",
                Email = "sarah@example.com",
                PhoneNumber = "555-123-4567",
                CreatedAt = DateTime.UtcNow
            };

            await _baseEntityRepository.AddCustomerEntityAsync(customer);
        }
    }
}
