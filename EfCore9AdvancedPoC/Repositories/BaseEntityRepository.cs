using EfCore9AdvancedPoC.Models.Inheritance;
using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models.Inheritance;
using Microsoft.EntityFrameworkCore;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class BaseEntityRepository : IBaseEntityRepository
    {
        private readonly AppDbContext _context;

        public BaseEntityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BaseEntity>> GetAllBaseEntitiesAsync()
        {
            return await _context.BaseEntities
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<EmployeeEntity>> GetAllEmployeeEntitiesAsync()
        {
            return await _context.EmployeeEntities.ToListAsync();
        }

        public async Task<List<CustomerEntity>> GetAllCustomerEntitiesAsync()
        {
            return await _context.CustomerEntities.ToListAsync();
        }

        public async Task<BaseEntity> AddEmployeeEntityAsync(EmployeeEntity employee)
        {
            _context.EmployeeEntities.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<BaseEntity> AddCustomerEntityAsync(CustomerEntity customer)
        {
            _context.CustomerEntities.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
