namespace EfCore9AdvancedPoCWithPostgres.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string Operation { get; set; }
        public DateTime Timestamp { get; set; }
    }
}