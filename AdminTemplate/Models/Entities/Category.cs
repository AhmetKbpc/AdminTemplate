using AdminTemplate.Models.Entities.Abstract;

namespace AdminTemplate.Models.Entities
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public IList<Product> Products { get; set; }
    }
}
