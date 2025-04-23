using System.Collections.Generic;

namespace EfCore9AdvancedPoCWithPostgres.Models.Relationships
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Navigation properties
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        
        // Skip navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
