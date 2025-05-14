# Entity Framework Core 9 Advanced Features Demo

This project demonstrates advanced features of Entity Framework Core 9 using PostgreSQL as the database provider. It showcases various data access patterns, optimization techniques, and relationship models that can be used in enterprise applications.

## Project Abstract

This proof of concept (PoC) application demonstrates the advanced capabilities of Entity Framework Core 9, focusing on performance optimization, complex data modeling, and efficient data access patterns. The project serves both as a learning resource and a reference implementation for developers looking to leverage EF Core 9's powerful features.

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

## Architecture and Design Patterns

### Repository Pattern Implementation

The application follows a clear layered architecture:



```
Client → Controllers → Services → Repositories → Database

```

#### Core Repository Interfaces

All repositories implement a common interface with standard CRUD operations:



```csharp
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
    Task<int> DeleteByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}

```

#### Entity-Specific Repositories

Each entity has its own specialized repository interface and implementation:

| Repository | Key Responsibilities |
|------------|---------------------|
| `IProductRepository` | Product management, shadow properties, and detailed product queries |
| `IUserRepository` | User management with soft delete support and newsletter preferences |
| `IOrderRepository` | Order management with JSON querying capabilities |
| `IEmployeeRepository` | Employee hierarchy management |
| `IAuditLogRepository` | Audit log querying by different criteria |
| `IBaseEntityRepository` | TPT inheritance entity management |
| `ITagRepository` | Tag management and product tag associations |

#### Service Layer

Services encapsulate business logic and orchestrate repository operations:

- `BulkOperationService`: Handles batch operations across multiple entities
- `DataSeedingService`: Manages database seeding with sample data

### Key Architecture Improvements

1. **Separation of Concerns**
   - Controllers focus on HTTP request/response handling
   - Repositories handle data access
   - Services implement business logic

2. **Improved Testability**
   - Repositories can be mocked for unit testing
   - Services can be tested independently from data access
   - Controllers can be tested with mocked service behavior

3. **Reduced Duplication**
   - Common data access patterns are implemented once
   - Business logic is centralized in services

4. **Transaction Management**
   - Services manage transactions across multiple repositories
   - Ensures data consistency for complex operations

5. **Entity Tracking**
   - Circular reference handling in repositories
   - Shadow property management

## Prerequisites

Before implementing this solution, you'll need:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [pgAdmin 4](https://www.pgadmin.org/download/) (optional, for database management)
- [Entity Framework Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)
- Basic knowledge of C#, .NET, and relational databases

## Getting Started

### 1. Configure the Connection String

The connection string is configured in `appsettings.json`:



```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=EfCore9PoC;Username=postgres;Password=A&n#K$MJS2dc"
    }
}

```

### 2. Apply Migrations

Run the following commands in the terminal from the project root directory:



```
dotnet ef migrations add InitialCreate
dotnet ef database update

```

### 3. Seed the Database

Use the built-in seeding endpoint to populate the database with sample data:



```
POST /api/demo/seed

```

### 4. Explore the API

Use Swagger UI (available at the root URL `/`) to explore and test the available endpoints.

## Project Structure

- **Controllers**:
  - `DemoController`: Basic EF Core features demonstrated using repositories
  - `AdvancedOperationsController`: Complex operations using repositories

- **Repositories**:
  - Repository interfaces: `IProductRepository`, `IUserRepository`, etc.
  - Repository implementations: `ProductRepository`, `UserRepository`, etc.
  - Common interface: `IRepository<T>`

- **Services**:
  - `BulkOperationService`: Cross-entity business operations 
  - `DataSeedingService`: Database seeding with test data

- **Models**:
  - Core entities (`Product`, `Order`, `User`, etc.)
  - Inheritance models (`BaseEntity`, `CustomerEntity`, `EmployeeEntity`)
  - Relationship entities (`ProductTag`, `Tag`)
  - JSON models (`OrderDetails`)
  - Owned types (`UserPreferences`, `ShippingAddress`)

- **Data**:
  - `AppDbContext`: Database context used only by repositories, not controllers

- **Interceptors**:
  - `AuditInterceptor`: Automatic audit logging

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
- `GET /api/advancedoperations/repository/products` - Gets all products using repository pattern
- `GET /api/advancedoperations/repository/products/{id}` - Gets product by ID using repository pattern
- `POST /api/advancedoperations/repository/products` - Creates product using repository pattern
- `PUT /api/advancedoperations/repository/products/{id}` - Updates product using repository pattern
- `DELETE /api/advancedoperations/repository/products/{id}` - Deletes product using repository pattern

## Troubleshooting Tips

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

#### Entity Tracking Issues

**Issue**: Circular reference errors in JSON serialization

**Solution**: 
Configure JSON serialization in Program.cs:



```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

```

#### Shadow Property Queries

**Issue**: Error translating DateTime.Subtract in shadow property queries

**Solution**:
Calculate the cutoff time before the query:



```csharp
// Instead of:
.Where(p => EF.Property<DateTime>(p, "LastViewedAt") > DateTime.UtcNow.Subtract(timeSpan))

// Use:
var cutoffTime = DateTime.UtcNow.AddTicks(-timeSpan.Ticks);
.Where(p => EF.Property<DateTime>(p, "LastViewedAt") > cutoffTime)

```

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

---

For questions or support, please open an issue on the repository or contact the project maintainer.
