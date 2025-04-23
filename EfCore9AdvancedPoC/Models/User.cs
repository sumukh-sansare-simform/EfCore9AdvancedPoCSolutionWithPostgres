using EfCore9AdvancedPoCWithPostgres.Models.Owned;

namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public UserPreferences Preferences { get; set; } = new();
        public bool IsDeleted { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
