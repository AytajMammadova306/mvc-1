using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia_self.ViewModels
{
    public class CreateSlideVM
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Describtion { get; set; }
        public int Order { get; set; }
        public IFormFile Photo { get; set; }
    }
}
