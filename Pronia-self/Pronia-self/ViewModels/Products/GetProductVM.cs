using Pronia_self.Models;

namespace Pronia_self.ViewModels
{
    public class GetProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        //Relational

        public string PrimaryImage { get; set; }
        public string SecondaryImage { get; set; }
    }
}
