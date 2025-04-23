// Add this to a new file: EfCore9AdvancedPoC/Models/Employee.cs
using System.Collections.Generic;

namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        
        // Self-referencing relationship
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
    }
}
