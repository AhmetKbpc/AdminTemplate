namespace AdminTemplate.Models.Entities.Abstract
{
    public abstract class BaseEntity<T> where T : IEquatable<T> //equatable: karşılaştırılabilir nesneler
    {
        public T Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedUser { get; set; }
    }
}
