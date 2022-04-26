using System.ComponentModel.DataAnnotations;

namespace MovieWebsiteMVC.Models.Data
{
    public class Anime
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        [Required, MaxLength(2500)]
        public string StoreLine { get; set; }
        [Required]
        public byte[] Poster { get; set; }
        public byte CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
