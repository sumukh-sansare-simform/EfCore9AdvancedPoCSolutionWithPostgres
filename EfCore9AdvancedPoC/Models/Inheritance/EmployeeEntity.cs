using EfCore9AdvancedPoCWithPostgres.Models.Inheritance;

namespace EfCore9AdvancedPoC.Models.Inheritance
{
    public class EmployeeEntity : BaseEntity
    {
        public string Department { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
    }
}
