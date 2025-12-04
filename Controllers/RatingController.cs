using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieSystem.Models;

namespace MovieSystem.Controllers
{
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int movieId)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == movieId);
            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.MovieId = movieId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Rating rating, int movieId)
        {
            if (!rating.Comment.IsNullOrEmpty())
            {
                rating.MovieId = movieId;
                _context.Ratings.Add(rating);
                _context.SaveChanges();
                return RedirectToAction("TopRated", "Movie");
            }

            ViewBag.MovieId = movieId;

            return View(rating);
        }
    }
}
