namespace Pronia_self.Models
{
    public class Color:BaseEntity
    {
        public string Name { get; set; }
        public List<ProductColor> ProductColor { get; set; }
    }
}
