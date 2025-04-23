using System;
using System.Collections.Generic;
using EfCore9AdvancedPoCWithPostgres.Models.Relationships;

namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Temporal table properties - with public setters
        public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.MaxValue;

        // Table splitting - shared primary key relationship
        public ProductDetail ProductDetail { get; set; }
        
        // Navigation collections
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        
        // Skip navigation for many-to-many
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
