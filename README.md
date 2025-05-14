# Entity Framework Core 9 Advanced Features Demo

This project demonstrates advanced features of Entity Framework Core 9 using PostgreSQL as the database provider. It showcases various data access patterns, optimization techniques, and relationship models that can be used in enterprise applications.

## Project Abstract

This proof of concept (PoC) application demonstrates the advanced capabilities of Entity Framework Core 9, focusing on performance optimization, complex data modeling, and efficient data access patterns. The project serves both as a learning resource and a reference implementation for developers looking to leverage EF Core 9's powerful features.

## What This PoC Is About

This project explores and demonstrates the most powerful and useful advanced features of Entity Framework Core 9. It's designed to showcase how modern data access patterns can be implemented using the latest EF Core capabilities. By examining this PoC, developers can learn practical approaches to solving common data access challenges in enterprise applications.

## When and Why to Use EF Core 9 with Advanced Features

EF Core 9 should be used when your application requires:

- **High performance data access**: Take advantage of compiled queries, split queries, and efficient bulk operations
- **Complex data models**: Model complex domain relationships with minimal code
- **Temporal data tracking**: Track historical changes without custom implementation
- **Advanced querying capabilities**: Use LINQ for type-safe queries with powerful execution strategies
- **Cross-cutting concerns**: Apply behaviors like auditing across your entire data access layer

The advanced features demonstrated in this PoC are particularly valuable when:

1. **Building enterprise applications** that require robust data access patterns
2. **Managing complex data relationships** beyond simple CRUD operations
3. **Optimizing performance** for high-throughput applications
4. **Tracking data changes** for audit or historical purposes
5. **Working with heterogeneous data** including relational and JSON structures

## Key Features Demonstrated

### Advanced Data Relationships

- **Many-to-Many Relationships**:
  - Implemented with a join entity (`ProductTag`) that contains additional metadata
  - Configured using skip navigations for simplified LINQ queries
  - Example: `Product ↔ Tag` relationship with assignment metadata

- **Table Splitting**:
  - `Product` and `ProductDetail` entities map to the same table
  - Allows logical separation of frequently vs. rarely accessed properties
  - Improves query performance by loading only needed data

- **Self-Referencing Relationships**:
  - `Employee` entity references itself for hierarchical data
  - Demonstrates management hierarchy with managers and direct reports
  - Uses `OnDelete(DeleteBehavior.Restrict)` to prevent orphaned records

- **TPT (Table-Per-Type) Inheritance**:
  - Base class `BaseEntity` with derived `CustomerEntity` and `EmployeeEntity`
  - Each type mapped to its own table with shared primary key
  - Allows polymorphic queries across the inheritance hierarchy

### Performance Optimization Techniques

- **Compiled Queries**:
  - Pre-compiled LINQ expressions for frequently executed queries
  - Eliminates query compilation overhead in repeated executions
  - Example: `GetUserByIdQuery` compiled once and reused

- **Split Queries**:
  - Separate SQL queries for complex entity graphs
  - Avoids cartesian explosion with multiple `Include()` statements
  - Example: Loading products with details and tags separately

- **Bulk Operations**:
  - Efficient batch processing with `ExecuteUpdate` and `ExecuteDelete`
  - Transaction management for consistency
  - Chunking large operations for better performance
  - Example: `BatchUpdateProductInventoryAsync`

- **Pagination**:
  - Efficient data retrieval with Skip/Take
  - Prevents loading excessive data into memory
  - Example: `GetPaginatedProductsAsync`

### Data Management Features

- **Temporal Tables**:
  - Tracks historical data changes with period columns (`ValidFrom` and `ValidTo`)
  - Enables point-in-time querying
  - Uses PostgreSQL's `infinity` timestamp for open-ended periods
  - Example: `QueryTemporalData` endpoint

- **Audit Logging**:
  - Automatic change tracking via `SaveChangesInterceptor`
  - Records entity type, operation type, and timestamp
  - No explicit code needed in business logic
  - Example: `AuditInterceptor` implementation

- **Soft Delete**:
  - Logical deletion with `IsDeleted` flag
  - Global query filters to automatically exclude deleted entities
  - `IgnoreQueryFilters()` to override when needed
  - Example: `User` entity with soft delete support

- **JSON Column Support**:
  - Native JSON storage in PostgreSQL
  - Strongly-typed access with C# models
  - JSON path querying through EF.Functions
  - Example: `OrderDetails` stored as JSONB

### Architecture Patterns

- **Repository Pattern**:
  - Clean separation of data access logic
  - Interface-based design for testability
  - Consistency in CRUD operations
  - Example: `IProductRepository` and implementation

- **Query Objects**:
  - Encapsulation of complex query logic
  - Reusable, testable query components
  - Example: `ProductsWithLowStockQuery`

- **Service Layer**:
  - Business logic encapsulation
  - Transaction management
  - Example: `BulkOperationService`

- **Interceptors**:
  - Cross-cutting concerns implementation
  - Automatic audit logging
  - Example: `AuditInterceptor`

## New in EF Core 9

This project demonstrates several features that are new or improved in EF Core 9:

- **Bulk Operations**: New ExecuteUpdate/ExecuteDelete APIs for efficient batch updates
- **JSON Column Improvements**: Enhanced JSON querying and mapping
- **Improved Temporal Query Support**: Better handling of temporal data
- **Enhanced Value Conversions**: Improved handling of custom value conversions
- **Performance Optimizations**: Better query translation and execution

## Prerequisites

Before implementing this solution, you'll need:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [pgAdmin 4](https://www.pgadmin.org/download/) (optional, for database management)
- [Entity Framework Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)
- Basic knowledge of C#, .NET, and relational databases

## How to Implement the Solution

### 1. Clone the Repository


```shell
git clone https://github.com/yourusername/EfCore9AdvancedPoC.git
cd EfCore9AdvancedPoC

```

### 2. PostgreSQL Setup

1. **Install PostgreSQL** if you haven't already
2. **Create a new database**:
   - Open pgAdmin or use the PostgreSQL command line
   - Create a new database named `EfCore9DemoDb`
   - Create a user with appropriate permissions or use the default `postgres` user


```sql
CREATE DATABASE "EfCore9DemoDb";

```

### 3. Configure the Connection String

Open `appsettings.json` and update the connection string to match your PostgreSQL installation:


```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EfCore9DemoDb;Username=postgres;Password=yourpassword;Include Error Detail=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}

```

### 4. Apply Migrations

Run the following commands in the terminal from the project root directory:


```shell
dotnet ef migrations add InitialCreate
dotnet ef database update

```

### 5. Seed the Database

Use the built-in seeding endpoint to populate the database with sample data:


```
POST /api/demo/seed

```

### 6. Run the Application


```shell
dotnet run

```

The application will start and be available at:
- API: https://localhost:5001
- Swagger UI: https://localhost:5001/index.html

## Project Structure

- **Controllers**:
  - `DemoController`: Basic EF Core features demonstration
  - `AdvancedOperationsController`: Complex scenarios using repository pattern

- **Data**:
  - `AppDbContext`: Database context with entity configurations

- **Models**:
  - Core entities (`Product`, `Order`, `User`, etc.)
  - Inheritance models (`BaseEntity`, `CustomerEntity`, `EmployeeEntity`)
  - Relationship entities (`ProductTag`, `Tag`)
  - JSON models (`OrderDetails`)
  - Owned types (`UserPreferences`, `ShippingAddress`)

- **Services**:
  - `BulkOperationService`: Business logic for batch operations

- **Repositories**:
  - `IProductRepository` & `ProductRepository`: Data access abstraction

- **Interceptors**:
  - `AuditInterceptor`: Automatic audit logging

```markdown
## Database Structure

This application uses a relational database with the following entity relationships:

### Core Entity Relationships


```
Users ───< Orders >─── Products ───< ProductDetails
                       │
                       └──< ProductTags >── Tags

```

### Employee Hierarchy and Inheritance Structure


```
Employees ───┬─< self (ManagerId)
             └─ BaseEntity (TPT) ──< EmployeeEntity / CustomerEntity

```

### Detailed Entity Relationships

#### Users and Orders
- `User` (1) → (Many) `Order`: One user can place many orders
- `User` contains `UserPreferences` as owned entity stored as JSON

#### Orders
- `Order` (Many) ← (1) `Product`: Each order references one product
- `Order` contains `OrderDetails` as JSON data
- `Order` includes `ShippingAddress` as owned entity 

#### Products
- `Product` (1) → (1) `ProductDetail`: Table splitting relationship with shared primary key
- `Product` (Many) ↔ (Many) `Tag`: Many-to-many relationship through `ProductTag` join entity
- `Product` includes temporal data tracking with `ValidFrom` and `ValidTo` columns

#### Employees
- `Employee` (Many) → (1) `Employee`: Self-referencing relationship for manager hierarchy
  - `ManagerId` in `Employee` references another `Employee`
  - `DirectReports` collection represents employees reporting to a manager

#### TPT (Table-Per-Type) Inheritance
- `BaseEntity`: Abstract base class with common properties
  - `EmployeeEntity`: Specialized employee data with department and position
  - `CustomerEntity`: Specialized customer data with encrypted email

#### Audit Logging
- `AuditLog`: Records entity changes through interceptors
  - Captures table name, operation type, and timestamp

### Database Schema Details

| Entity          | PK/FK Relationship | Notes                                           |
|-----------------|--------------------|-------------------------------------------------|
| Users           | PK: Id             | Includes soft delete flag and JSON preferences   |
| Orders          | PK: Id, FK: UserId, ProductId | Contains JSON order details          |
| Products        | PK: Id             | Includes temporal data tracking                 |
| ProductDetails  | PK/FK: ProductId   | Table splitting with Product                    |
| Tags            | PK: Id             | Product categorization                          |
| ProductTags     | PK: ProductId+TagId| Junction table with metadata                    |
| Employees       | PK: Id, FK: ManagerId | Self-referencing relationship               |
| BaseEntities    | PK: Id             | Base table for inheritance                      |
| EmployeeEntities| PK/FK: Id          | TPT inheritance from BaseEntity                |
| CustomerEntities| PK/FK: Id          | TPT inheritance with encrypted email            |
| AuditLogs       | PK: Id             | Automatic change tracking                       |

### Special Features

- **JSON Storage**: User preferences and order details stored as JSON
- **Table Splitting**: Product and ProductDetail entities map to same table
- **TPT Inheritance**: Base and derived entities in separate tables with shared key
- **Self-Referencing**: Employee hierarchy with manager references
- **Many-to-Many**: Products and Tags with additional metadata in junction table
- **Column Encryption**: Customer email encrypted using AES
- **Soft Delete**: Users have IsDeleted flag with global query filter
- **Temporal Data**: Products track history with period columns
- **Shadow Properties**: LastViewedAt tracked but not in entity class

```

This comprehensive database structure section clearly outlines all the relationships in your application, including the one-to-many, many-to-many, inheritance, and special modeling features. The diagram follows your requested format and provides useful details about the data model design.

## Required Configuration Details

### Database Schema Configuration

The application uses EF Core's code-first approach to define the database schema. Entity configurations are centralized in the `AppDbContext.OnModelCreating` method:

- **Temporal Tables**: Configure `Product` entity with period columns
- **JSON Columns**: Configure `User.Preferences` and `Order.Details` as JSON
- **TPT Inheritance**: Map inheritance hierarchy to separate tables
- **Global Query Filters**: Configure soft delete behavior for `User` entity

### PostgreSQL Configuration

PostgreSQL-specific features are configured in `AppDbContext.OnConfiguring`:

- **Dynamic JSON**: Enable support for dynamic JSON operations
- **Error Details**: Include error details in exceptions for development
- **Temporal Data**: Configure infinity timestamp handling

### Logging Configuration

Logging is configured in `appsettings.json` with different levels for application code and EF Core:


```json
"Logging": {
  "LogLevel": {
    "Default": "Information", 
    "Microsoft.AspNetCore": "Warning",
    "Microsoft.EntityFrameworkCore": "Information"
  }
}

```

### Security Configuration

Column encryption is configured for sensitive data:


```csharp
modelBuilder.Entity<CustomerEntity>()
    .Property(c => c.Email)
    .HasConversion(
        v => _encryptionProvider.Encrypt(Encoding.UTF8.GetBytes(v ?? string.Empty)),
        v => Encoding.UTF8.GetString(_encryptionProvider.Decrypt(v) ?? Array.Empty<byte>()));

```

## API Endpoints

### Demo Controller

- `POST /api/demo/seed` - Seeds the database with sample data
- `GET /api/demo/json-query` - Demonstrates JSON querying
- `POST /api/demo/bulk-operations` - Runs batch operations on data
- `GET /api/demo/temporal-query/{productId}` - Demonstrates temporal data querying
- `GET /api/demo/audit-logs` - Retrieves audit logs
- `GET /api/demo/tpt-inheritance` - Demonstrates TPT inheritance
- `GET /api/demo/soft-delete` - Demonstrates soft delete functionality
- `GET /api/demo/compiled-query` - Demonstrates compiled queries
- `GET /api/demo/split-query` - Demonstrates split queries
- `GET /api/demo/shadow-properties` - Demonstrates shadow properties
- `GET /api/demo/change-tracking` - Demonstrates change tracking
- `GET /api/demo/self-referencing` - Demonstrates self-referencing relationships
- `GET /api/demo/table-splitting` - Demonstrates table splitting

### Advanced Operations Controller

- `GET /api/advancedoperations/health-check` - Checks database connectivity
- `GET /api/advancedoperations/product-details` - Gets products with optional related data
- `GET /api/advancedoperations/products/{page}` - Gets paginated products
- `POST /api/advancedoperations/batch-inventory-update` - Updates product inventories in batch
- `GET /api/advancedoperations/low-stock` - Gets products with low stock
- `GET /api/advancedoperations/repository/products` - Gets all products using repository pattern
- `GET /api/advancedoperations/repository/products/{id}` - Gets product by ID using repository pattern
- `POST /api/advancedoperations/repository/products` - Creates product using repository pattern
- `PUT /api/advancedoperations/repository/products/{id}` - Updates product using repository pattern
- `DELETE /api/advancedoperations/repository/products/{id}` - Deletes product using repository pattern

## Testing the API

### Using Swagger UI

The easiest way to test the API is to use the built-in Swagger UI available at the root URL (`/` or `/index.html`) when running the application.

### Using the HTTP File

The project includes an HTTP file (`EfCore9AdvancedPoC.http`) that you can use with Visual Studio's REST Client or with the REST Client extension in Visual Studio Code.

### Using Postman

You can also use Postman to test the API. Here are some example requests:

#### Create a Product


```json
POST /api/advancedoperations/repository/products
Content-Type: application/json

{
  "name": "Gaming Laptop",
  "quantity": 15,
  "price": 1499.99,
  "createdAt": "2025-04-22T12:00:00Z",
  "updatedAt": "2025-04-22T12:00:00Z",
  "validFrom": "2025-04-22T12:00:00Z",
  "validTo": "9999-12-31T23:59:59.9999999Z",
  "productDetail": {
    "description": "High-performance gaming laptop with RGB keyboard",
    "specifications": "Intel i9, 32GB RAM, RTX 4080, 2TB SSD",
    "manufacturer": "TechGaming",
    "imageUrl": "/images/gaming-laptop.jpg"
  }
}

```

#### Update a Product


```json
PUT /api/advancedoperations/repository/products/1
Content-Type: application/json

{
  "name": "Updated Gaming Laptop Pro",
  "quantity": 12,
  "price": 1699.99,
  "createdAt": "2025-04-22T12:00:00Z",
  "updatedAt": "2025-04-22T12:30:00Z",
  "validFrom": "2025-04-22T12:00:00Z",
  "validTo": "9999-12-31T23:59:59.9999999Z",
  "productDetail": {
    "description": "Enhanced gaming laptop with premium features",
    "specifications": "Intel i9 12th Gen, 64GB RAM, RTX 4090, 4TB SSD",
    "manufacturer": "TechGaming Pro",
    "imageUrl": "/images/gaming-laptop-pro.jpg"
  }
}

```

#### Batch Inventory Update


```json
POST /api/advancedoperations/batch-inventory-update
Content-Type: application/json

{
  "1": 5,
  "2": -3,
  "3": 10
}

```

## Real-World Use Cases

### E-commerce Platform

The PoC models a simplified e-commerce system with:

- **Product Catalog**: Products with detailed specifications and tags
- **Inventory Management**: Stock tracking with low-stock alerts
- **Order Processing**: Orders with JSON-based details and shipping information
- **Customer Management**: User profiles with preferences

Real-world applications include:
- Product inventory tracking and automated reordering
- Historical price analysis using temporal data
- Category-based product navigation using many-to-many relationships
- Order history tracking with detailed JSON data

### Enterprise Resource Planning

The hierarchical employee model demonstrates:

- **Organizational Structure**: Self-referencing relationships for reporting hierarchy
- **Role-Based Access**: Different entity types for customers and employees
- **Audit Trails**: Automatic tracking of data changes

Real-world applications include:
- Organizational chart visualization
- Permission inheritance through management hierarchy
- Historical record keeping for compliance

### Content Management System

The flexible data structures demonstrate:

- **Flexible Content Types**: Using JSON columns for varied content structures
- **Content Tagging**: Many-to-many relationships with metadata
- **Content Versioning**: Temporal data for tracking changes

Real-world applications include:
- Managing diverse content types with a single data model
- Point-in-time content restoration
- Content categorization and filtering

## Technical Implementation Details

### Entity Framework Core 9 Features Explained

#### Temporal Tables Implementation

Temporal tables are configured in `AppDbContext.OnModelCreating`:


```csharp
modelBuilder.Entity<Product>()
    .ToTable("Products", b => b.IsTemporal(t => {
        t.HasPeriodStart("ValidFrom");
        t.HasPeriodEnd("ValidTo");
        t.UseHistoryTable("ProductsHistory");
    }));

```

PostgreSQL doesn't have built-in temporal table support like SQL Server, so we implement it using period columns:


```csharp
modelBuilder.Entity<Product>(entity =>
{
    // Set ValidFrom default constraint
    entity.Property(p => p.ValidFrom)
        .HasDefaultValueSql("CURRENT_TIMESTAMP")
        .IsRequired();

    // Set ValidTo default constraint
    entity.Property(p => p.ValidTo)
        .HasDefaultValueSql("'infinity'::timestamp")
        .IsRequired();
});

```

#### JSON Column Mapping

JSON columns are configured for both owned entities and regular properties:


```csharp
// JSON column mapping
modelBuilder.Entity<User>().OwnsOne(u => u.Preferences).ToJson();
modelBuilder.Entity<Order>().Property(o => o.Details).HasColumnType("jsonb");

```

JSON queries are performed using EF.Functions:


```csharp
var electronicsOrders = await _context.Orders
    .Where(o => EF.Functions.JsonContains(
        EF.Property<string>(o, "Details"),
        @"{""Tags"": [""electronics""]}"
    ))
    .ToListAsync();

```

#### Compiled Queries

Compiled queries are created once and reused for better performance:


```csharp
var getUserByIdQuery = EF.CompileQuery(
    (AppDbContext context, int id) =>
        context.Users
            .Include(u => u.Orders)
            .FirstOrDefault(u => u.Id == id));

// Usage
var user = getUserByIdQuery(_context, userId);

```

#### Shadow Properties

Shadow properties are properties that exist in the database but not in the entity class:


```csharp
modelBuilder.Entity<Product>()
    .Property<DateTime>("LastViewedAt")
    .HasDefaultValue(DateTime.UtcNow);

```

Usage:


```csharp
_context.Entry(product).Property("LastViewedAt").CurrentValue = DateTime.UtcNow;

```

#### Global Query Filters

Global query filters automatically apply conditions to queries:


```csharp
modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

```

#### Interceptors

Save changes interceptor for audit logging:

## Troubleshooting Steps

### Common Issues and Solutions

#### Migration Errors

**Issue**: `An error occurred while creating/accessing the database.`

**Solution**:
1. Verify PostgreSQL connection string in `appsettings.json`
2. Ensure the database user has sufficient permissions
3. Check if PostgreSQL service is running
4. Try recreating migrations:
   

```
   dotnet ef migrations remove
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   

```

#### Circular Reference Errors

**Issue**: `A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth.`

**Solution**:
1. Use DTOs for API responses instead of entity objects
2. Configure JSON serialization in Program.cs:
   

```csharp
   builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
       });
   

```

#### Entity Creation/Update Errors

**Issue**: `The Product field is required` when creating/updating nested entities

**Solution**:
1. Use the DTO approach for API requests
2. Set circular references manually in your repository or controller:
   

```csharp
   product.ProductDetail.Product = product;
   

```

#### PostgreSQL Temporal Data Issues

**Issue**: `DateTime.MaxValue` serializing as verbose timestamp instead of "infinity"

**Solution**:
1. Use custom JSON converters
2. Implement custom mapping in API responses:
   

```csharp
   ValidTo = product.ValidTo >= DateTime.MaxValue.AddDays(-1) ? "infinity" : product.ValidTo
   

```

#### Performance Issues

**Issue**: Slow query performance with complex object graphs

**Solution**:
1. Use `AsSplitQuery()` for queries with multiple includes
2. Add appropriate indexes to the database
3. Use compiled queries for frequently executed operations
4. Implement pagination for large result sets
5. Use projection queries to select only needed fields

## Performance Considerations

- **Split Queries**: Use for loading complex object graphs to avoid cartesian explosion
- **Compiled Queries**: Use for frequently executed queries
- **Bulk Operations**: Use ExecuteUpdate/ExecuteDelete for set-based operations
- **Pagination**: Always implement for user-facing listing operations
- **Selective Loading**: Only load related entities when needed

## Security Considerations

- **Column Encryption**: Sensitive data is encrypted using AES encryption
- **Data Validation**: Input validation is performed at the controller level
- **Transaction Management**: Ensures data consistency during batch operations

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Future Enhancements

- Add user authentication and authorization
- Implement advanced caching strategies
- Add GraphQL support for flexible querying
- Demonstrate integration with Azure Cosmos DB
- Add performance benchmarks

---

For questions or support, please open an issue on the repository or contact the project maintainer.
