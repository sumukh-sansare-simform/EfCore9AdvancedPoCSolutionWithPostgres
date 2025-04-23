namespace EfCore9AdvancedPoCWithPostgres.Models.Inheritance
{
    public class CustomerEntity : BaseEntity
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
