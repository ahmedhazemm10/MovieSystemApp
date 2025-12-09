using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieSystem.Models;

namespace MovieSystem.Controllers
{
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ActorController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int pageN)
        {
            var pages = Math.Ceiling((_context.Actors.Count() / 5.0));
            if (pageN < 1 || pageN > pages)
            {
                pageN = 1;
            }
            ViewBag.Pages = pages;
            ViewBag.CurrentPage = pageN;
            var actors = _context.Actors.Skip((pageN - 1) * 5).Take(5).ToList();
            return View(actors);
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Actors.Add(actor);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.FirstOrDefault(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Actors.Update(actor);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var actor = _context.Actors.FirstOrDefault(y => y.Id == id);
            if (actor is not null)
            {
                _context.Actors.Remove(actor);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Delete",actor);
        }
        public IActionResult Search(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return View("Index",_context.Actors);
            }
            name = name.ToLower();
            var actors = _context.Actors.Where(a => a.Name.ToLower().Contains(name)).ToList();
            return View("Index", actors);
        }
        public IActionResult Sort()
        {
            var actors = _context.Actors.OrderBy(a => a.Name).ThenBy(a => a.Age).ToList();
            return View("Index",actors);
        }

        public IActionResult Details(int id)
        {
            var actorFilms = _context.Actors.Include(a => a.Movies).FirstOrDefault(a => a.Id == id);
            if (actorFilms == null)
            {
                return NotFound();
            }
            return View(actorFilms);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddMovie(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor == null) return NotFound();

            ViewBag.ActorId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddMovie(int id, string title)
        {
            var actor = _context.Actors.Include(a => a.Movies).FirstOrDefault(a => a.Id == id);
            var movie = _context.Movies.FirstOrDefault(a => a.Title.ToLower() == title.ToLower());
            if (actor == null || movie == null)
            {
                return NotFound();
            }
            if (actor.Movies.Any(m => m.Title == title))
            {
                return BadRequest();
            }
            actor.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction("Details", new { Id = id });
        }
    }
}
