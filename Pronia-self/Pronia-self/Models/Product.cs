namespace Pronia_self.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        //Relational

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductImage> ProductImages { get; set; }
    }
}
