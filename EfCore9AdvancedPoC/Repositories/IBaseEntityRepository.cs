using EfCore9AdvancedPoC.Models.Inheritance;
using EfCore9AdvancedPoCWithPostgres.Models.Inheritance;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IBaseEntityRepository
    {
        Task<List<BaseEntity>> GetAllBaseEntitiesAsync();
        Task<List<EmployeeEntity>> GetAllEmployeeEntitiesAsync();
        Task<List<CustomerEntity>> GetAllCustomerEntitiesAsync();
        Task<BaseEntity> AddEmployeeEntityAsync(EmployeeEntity employee);
        Task<BaseEntity> AddCustomerEntityAsync(CustomerEntity customer);
    }
}
