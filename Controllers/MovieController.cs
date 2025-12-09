using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieSystem.Models;

namespace MovieSystem.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int pageN = 1)
        {
            var pages = Math.Ceiling((_context.Movies.Count() / 5.0));
            if (pageN < 1 || pageN > pages)
            {
                pageN = 1;
            }
            ViewBag.Pages = pages;
            ViewBag.CurrentPage = pageN;
            var movies = _context.Movies.Skip((pageN - 1) * 5).Take(5);
            return View(movies);
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Movie movie)
        {
            if(ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        [HttpPost]
        public IActionResult Edit(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Update(movie);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = (_context.Movies.FirstOrDefault(m => m.Id == id));
            if (movie is not null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Delete", movie);
        }
        public IActionResult Search(string title, string genre, int year)
        {
            var movies = new List<Movie>();
            if(!title.IsNullOrEmpty())
            {
                title = title.ToLower();
                movies = _context.Movies.Where(m => m.Title.ToLower().Contains(title)).ToList();
            }
            if (!genre.IsNullOrEmpty())
            {
                genre = genre.ToLower();
                if (movies.Count > 0)
                {
                    movies = movies.Where(m => m.Genre.ToLower().Contains(genre)).ToList();
                }
                else
                {
                    if(!title.IsNullOrEmpty())
                    {
                        return NotFound();
                    }
                    movies = _context.Movies.Where(m => m.Genre.ToLower().Contains(genre)).ToList();
                }
            }
            if (year > 0)
            {
                if (movies.Count > 0)
                {
                    movies = movies.Where(m => m.ReleaseYear == year).ToList();
                }
                else
                {
                    if (!genre.IsNullOrEmpty())
                    {
                        return NotFound();
                    }
                    movies = _context.Movies.Where(m => m.ReleaseYear == year).ToList();
                }
            }
            return View("Index", movies);
        }
        public IActionResult Sort()
        {
            var movies = _context.Movies.OrderBy(m => m.ReleaseYear).ToList();
            return View("Index", movies);
        }
        public IActionResult SortByTitle()
        {
            var movies = _context.Movies.OrderBy(m => m.Title).ToList();
            return View("Index", movies);
        }
        public IActionResult SortByYear()
        {
            var movies = _context.Movies.OrderBy(m => m.ReleaseYear).ToList();
            return View("Index", movies);
        }
        public IActionResult TopRated()
        {
            var movies = _context.Movies.GroupJoin(_context.Ratings, m => m.Id, r => r.MovieId, (m, r) => new MovieRate()
            {
                MovieName = m.Title,
                AVGRate = r.Any() ? Math.Round(r.Average(r => r.Stars),2) : 0,
                ImageUrl = m.ImageUrl
            }).OrderByDescending(mr => mr.AVGRate).Take(3);
            return View(movies);
        }
        public IActionResult Details(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if ( movie is null)
            {
                return NotFound();
            }
            var movieDetails = _context.Movies.Include(m => m.Ratings).ThenInclude(r => r.User).Select((m) => new MovieDetails()
            {
                Description = m.Description,
                Genre = m.Genre,
                ReleaseYear = m.ReleaseYear,
                ImageUrl = m.ImageUrl,
                Rate = m.Ratings.Any() ? m.Ratings.Average(r => r.Stars) : 0,
                Id = m.Id,
                Reviews = m.Ratings.Select(r => new ReviewDTO()
                {
                    Comment = r.Comment,
                    UserName = r.User.UserName
                }).ToList()
            }).FirstOrDefault(m => m.Id == id);
            return View(movieDetails);
        }
    }
}
