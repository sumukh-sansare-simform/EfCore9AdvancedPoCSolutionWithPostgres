using System;
using EfCore9AdvancedPoCWithPostgres.Models.Owned;
using EfCore9AdvancedPoCWithPostgres.Models.Json;

namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
        public OrderDetails Details { get; set; } = new(); // JSON column

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public ShippingAddress ShippingAddress { get; set; } = new();
    }
}
