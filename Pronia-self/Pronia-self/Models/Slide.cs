using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia_self.Models
{
    public class Slide:BaseEntity
    {
        [MaxLength(50,ErrorMessage ="50 den cox charakter olmaz")]
        [MinLength(2)]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Describtion { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        [NotMapped]

        public IFormFile Photo { get; set; }
    }
}
