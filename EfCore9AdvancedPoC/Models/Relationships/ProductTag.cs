using System;

namespace EfCore9AdvancedPoCWithPostgres.Models.Relationships
{
    public class ProductTag
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        
        // Additional join table properties
        public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
        public string AssignedBy { get; set; }
    }
}
