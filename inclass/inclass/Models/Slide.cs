namespace inclass.Models
{
    public class Slide:BaseEntity
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
    }
}
