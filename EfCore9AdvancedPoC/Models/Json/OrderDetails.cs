namespace EfCore9AdvancedPoCWithPostgres.Models.Json
{
    public class OrderDetails
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string[] Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}