using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieSystem.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public Movie Movie { get; set; }
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
    }
}
