using Microsoft.EntityFrameworkCore;

namespace EfCore9AdvancedPoCWithPostgres.Models.Owned
{
    [Owned]
    public class ShippingAddress
    {
        public string Line1 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }


}
