using EfCore9AdvancedPoCWithPostgres.Models.Relationships;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<List<Tag>> GetTagsForProductAsync(int productId);
        Task AddTagToProductAsync(int productId, int tagId, string assignedBy);
        Task RemoveTagFromProductAsync(int productId, int tagId);
    }
}
