namespace MovieSystem.Models
{
    public class MovieDetails
    {
        public int Id { get; set; }
        public int ReleaseYear { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public double Rate { get; set; }
        public string ImageUrl { get; set; }
    }
}
