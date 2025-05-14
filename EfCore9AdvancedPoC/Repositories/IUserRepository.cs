using EfCore9AdvancedPoCWithPostgres.Models;

namespace EfCore9AdvancedPoCWithPostgres.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetAllIncludingDeletedAsync();
        Task<User> GetWithOrdersAsync(int id);
        Task<int> SoftDeleteAsync(int id);
        Task<int> OptInAllToNewsletterAsync();
    }
}
