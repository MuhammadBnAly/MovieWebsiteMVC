using MovieWebsiteMVC.Models.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsiteMVC.ViewModels
{
    public class AnimeFormViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }
        [Range(1,10)]
        public double Rate { get; set; }
        [Required, StringLength(2500)]
        public string StoreLine { get; set; }
        [Display(Name = "Select Poster...")]
        public byte[] Poster { get; set; }
        [Display(Name ="Category")]
        public byte CategoryId { get; set; }
        public IEnumerable<Category> Categories { get; set; }

    }
}
