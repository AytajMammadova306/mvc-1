namespace Pronia_self.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        //Relational
        public List<Product> Products { get; set; }

    }
}
