using System;

namespace EfCore9AdvancedPoCWithPostgres.Models.Inheritance
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
