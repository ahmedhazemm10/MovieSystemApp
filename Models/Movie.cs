using System.ComponentModel.DataAnnotations;

namespace MovieSystem.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public int ReleaseYear { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
    }
}
