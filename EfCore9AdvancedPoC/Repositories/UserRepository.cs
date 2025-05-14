using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }

        public async Task<User> AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
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
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<User>> GetAllIncludingDeletedAsync()
        {
            return await _context.Users.IgnoreQueryFilters().ToListAsync();
        }

        public async Task<User> GetWithOrdersAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<int> SoftDeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return 0;

            user.IsDeleted = true;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> OptInAllToNewsletterAsync()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                user.Preferences.ReceiveNewsletter = true;
            }
            return await _context.SaveChangesAsync();
        }
    }
}
