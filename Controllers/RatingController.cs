using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rating rating, int movieId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!rating.Comment.IsNullOrEmpty())
            {
                rating.MovieId = movieId;
                rating.UserId = currentUserId;

                _context.Ratings.Add(rating);
                _context.SaveChanges();
                return RedirectToAction("TopRated", "Movie");
            }

            ViewBag.MovieId = movieId;
            return View(rating);
        }
    }
}