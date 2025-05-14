using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<List<Employee>> FindAsync(Expression<Func<Employee, bool>> predicate)
        {
            return await _context.Employees.Where(predicate).ToListAsync();
        }

        public async Task<Employee> AddAsync(Employee entity)
        {
            await _context.Employees.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(Employee entity)
        {
            _context.Employees.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Employee entity)
        {
            _context.Employees.Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return 0;
            return await DeleteAsync(entity);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<List<Employee>> GetOrganizationalHierarchyAsync()
        {
            return await _context.Employees
                .Include(e => e.DirectReports)
                .Where(e => e.Manager == null)
                .ToListAsync();
        }

        public async Task<Employee> GetByPositionAsync(string position)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Position == position);
        }

        public async Task<List<Employee>> GetDirectReportsAsync(int managerId)
        {
            return await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
        }
    }
}
