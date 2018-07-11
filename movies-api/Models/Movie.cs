using System;
using System.ComponentModel.DataAnnotations;

namespace movies_api.Models
{
    public class Movie
    {
        public int ID { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [StringLength(50)]
        [Required]
        public string Genre { get; set; }

        [DataType(DataType.Currency)]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [StringLength(20)]
        [Required]
        public string Rating { get; set; }
    }
}