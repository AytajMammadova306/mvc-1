using inclass.Models;

namespace inclass.ViewModels
{
    public class DetailsVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProduct { get; set; }

    }
}
