using System;

namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class ProductDetail
    {
        // This property serves as both primary key and foreign key to Product
        public int ProductId { get; set; }
        
        public string Description { get; set; }
        public string Specifications { get; set; }
        public string Manufacturer { get; set; }
        public string ImageUrl { get; set; }
        
        // Navigation property to Product (inverse)
        public Product Product { get; set; }
    }
}
