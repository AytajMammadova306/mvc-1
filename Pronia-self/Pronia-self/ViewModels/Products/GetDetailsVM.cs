using Pronia_self.Models;

namespace Pronia_self.ViewModels
{
    public class GetDetailsVM
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> Colors { get; set; }
    }
}
