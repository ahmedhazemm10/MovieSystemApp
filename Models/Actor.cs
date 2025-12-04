using System.ComponentModel.DataAnnotations;

namespace MovieSystem.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public int Age { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
