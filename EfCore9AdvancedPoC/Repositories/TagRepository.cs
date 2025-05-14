using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<List<Tag>> FindAsync(Expression<Func<Tag, bool>> predicate)
        {
            return await _context.Tags.Where(predicate).ToListAsync();
        }

        public async Task<Tag> AddAsync(Tag entity)
        {
            await _context.Tags.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(Tag entity)
        {
            _context.Tags.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Tag entity)
        {
            _context.Tags.Remove(entity);
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
            return await _context.Tags.AnyAsync(t => t.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Tags.CountAsync();
        }

        public async Task<List<Tag>> GetTagsForProductAsync(int productId)
        {
            return await _context.ProductTags
                .Where(pt => pt.ProductId == productId)
                .Select(pt => pt.Tag)
                .ToListAsync();
        }

        public async Task AddTagToProductAsync(int productId, int tagId, string assignedBy)
        {
            var productTag = new ProductTag
            {
                ProductId = productId,
                TagId = tagId,
                AssignedOn = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            await _context.ProductTags.AddAsync(productTag);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTagFromProductAsync(int productId, int tagId)
        {
            var productTag = await _context.ProductTags
                .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);

            if (productTag != null)
            {
                _context.ProductTags.Remove(productTag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
