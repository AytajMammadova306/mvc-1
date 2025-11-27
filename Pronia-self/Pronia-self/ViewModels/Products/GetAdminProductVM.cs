using Pronia_self.Models;

namespace Pronia_self.ViewModels
{
    public class GetAdminProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string Image { get; set; }
    }
}
