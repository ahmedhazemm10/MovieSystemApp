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
        public IActionResult Index()
        {
            return View(_context.Movies.ToList());
        }

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
                AVGRate = r.Any() ? r.Average(r => r.Stars) : 0,
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
            var movieDetails = _context.Movies.GroupJoin(_context.Ratings, m => m.Id, r => r.MovieId, (m, r) => new MovieDetails()
            {
                Title = m.Title,
                Rate = r.Any() ? r.Average(r => r.Stars) : 0,
                ImageUrl = m.ImageUrl,
                Description = m.Description,
                Genre = m.Genre,
                ReleaseYear = m.ReleaseYear,
                Id = m.Id
            }).ToList().FirstOrDefault(m  => m.Id == id);
            return View(movieDetails);
        }
    }
}
