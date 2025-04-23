using Microsoft.EntityFrameworkCore;

namespace EfCore9AdvancedPoCWithPostgres.Models.Owned
{
    [Owned]
    public class UserPreferences
    {
        public string Theme { get; set; }
        public bool ReceiveNewsletter { get; set; }
    }

}
