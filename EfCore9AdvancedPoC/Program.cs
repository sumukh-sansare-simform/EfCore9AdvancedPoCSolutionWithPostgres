using System.Text.Json.Serialization;
using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Interceptors;
using EfCore9AdvancedPoCWithPostgres.Repositories;
using EfCore9AdvancedPoCWithPostgres.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<BulkOperationService>();

// Register Repository and Query classes
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Fixed: Single AddControllers call with all options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization options
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;

        // Add this line to handle circular references
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EF Core 9 Advanced PoC API",
        Version = "v1",
        Description = "Proof of Concept for EF Core 9 Advanced Features"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Add Swagger middleware
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EF Core 9 Advanced PoC v1");
        c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations at startup in development environments
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Comment out or remove this line if you're using Option 2 above
        dbContext.Database.Migrate();
    }
}

app.Run();
