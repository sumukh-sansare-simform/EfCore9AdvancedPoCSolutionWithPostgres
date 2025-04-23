namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        // Nested ProductDetail data without circular reference
        public ProductDetailDto ProductDetail { get; set; }

        public class ProductDetailDto
        {
            public string Description { get; set; }
            public string Specifications { get; set; }
            public string Manufacturer { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
