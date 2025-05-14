using EfCore9AdvancedPoCWithPostgres.Models;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<Employee>> GetOrganizationalHierarchyAsync();
        Task<Employee> GetByPositionAsync(string position);
        Task<List<Employee>> GetDirectReportsAsync(int managerId);
    }
}
