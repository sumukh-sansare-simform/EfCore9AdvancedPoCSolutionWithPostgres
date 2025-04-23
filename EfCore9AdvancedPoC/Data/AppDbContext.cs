using System.Security.Cryptography;
using System.Text;
using EfCore9AdvancedPoC.Models.Inheritance;
using EfCore9AdvancedPoCWithPostgres.Models.Relationships;
using EfCore9AdvancedPoCWithPostgres.Interceptors;
using EfCore9AdvancedPoCWithPostgres.Models;
using EfCore9AdvancedPoCWithPostgres.Models.Inheritance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Npgsql;

namespace EfCore9AdvancedPoCWithPostgres.Data
{
    public class AppDbContext : DbContext
    {
        private readonly AuditInterceptor _auditInterceptor;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly string _connectionString;

        // DbSet properties
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<BaseEntity> BaseEntities => Set<BaseEntity>();
        public DbSet<CustomerEntity> CustomerEntities => Set<CustomerEntity>();
        public DbSet<EmployeeEntity> EmployeeEntities => Set<EmployeeEntity>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<ProductTag> ProductTags => Set<ProductTag>();
        public DbSet<Employee> Employees => Set<Employee>();

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            AuditInterceptor auditInterceptor,
            IConfiguration configuration) : base(options)
        {
            _auditInterceptor = auditInterceptor;
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            // Encryption setup for AES
            byte[] encryptionKey = new byte[32]; // 32 bytes for AES-256
            byte[] encryptionIV = new byte[16]; // 16 bytes for AES IV

            // Key derivation with Rfc2898DeriveBytes for encryption
            using (var deriveBytes = new Rfc2898DeriveBytes("YourSecretPassphrase",
                Encoding.UTF8.GetBytes("YourSaltValue"), 1000, HashAlgorithmName.SHA256))
            {
                Buffer.BlockCopy(deriveBytes.GetBytes(32), 0, encryptionKey, 0, 32);
                Buffer.BlockCopy(deriveBytes.GetBytes(16), 0, encryptionIV, 0, 16);
            }

            _encryptionProvider = new AesProvider(encryptionKey, encryptionIV);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);

            // Enable sensitive data logging for development
            optionsBuilder.EnableSensitiveDataLogging();

            // Use query tracking behavior
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

            // Suppress model changes warning
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            optionsBuilder.UseNpgsql(dataSource);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // JSON column mapping for User Preferences and Order Details
            modelBuilder.Entity<User>().OwnsOne(u => u.Preferences).ToJson();
            modelBuilder.Entity<Order>().Property(o => o.Details).HasColumnType("jsonb");

            // Complex Type / Owned Entity: Order - User relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired(false);  // Make relationship optional

            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.User.IsDeleted);

            // Employee - Manager relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Shadow property for Product LastViewedAt
            modelBuilder.Entity<Product>()
                .Property<DateTime>("LastViewedAt")
                .HasDefaultValue(DateTime.UtcNow);

            // Temporal tables for Product entity
            modelBuilder.Entity<Product>()
                .ToTable("Products", b => b.IsTemporal(t =>
                {
                    t.HasPeriodStart("ValidFrom");
                    t.HasPeriodEnd("ValidTo");
                    t.UseHistoryTable("ProductsHistory");
                }));

            // ValidFrom and ValidTo columns for temporal tables
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.ValidFrom)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                entity.Property(p => p.ValidTo)
                    .HasDefaultValueSql("'infinity'::timestamp")
                    .IsRequired();
            });

            // Product-ProductDetail one-to-one relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductDetail)
                .WithOne(pd => pd.Product)
                .HasForeignKey<ProductDetail>(pd => pd.ProductId);

            // Global query filter for User soft delete
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

            // Table-Per-Type (TPT) inheritance configuration
            modelBuilder.Entity<BaseEntity>().ToTable("BaseEntities");
            modelBuilder.Entity<CustomerEntity>().ToTable("CustomerEntities");
            modelBuilder.Entity<EmployeeEntity>().ToTable("EmployeeEntities");

            // Column encryption for CustomerEntity email
            modelBuilder.Entity<CustomerEntity>()
                .Property(c => c.Email)
                .HasConversion(
                    v => _encryptionProvider.Encrypt(Encoding.UTF8.GetBytes(v ?? string.Empty)),
                    v => Encoding.UTF8.GetString(_encryptionProvider.Decrypt(v) ?? Array.Empty<byte>()));

            // Many-to-many relationship configuration for ProductTag
            modelBuilder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId });

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId);

            // Skip navigation for many-to-many relationship
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Products)
                .UsingEntity<ProductTag>();


            // Table splitting example for ProductDetail
            modelBuilder.Entity<ProductDetail>()
                .HasKey(pd => pd.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductDetail)
                .WithOne(pd => pd.Product)
                .HasForeignKey<ProductDetail>(pd => pd.ProductId);

            modelBuilder.Entity<Product>()
                .Navigation(p => p.ProductDetail)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
