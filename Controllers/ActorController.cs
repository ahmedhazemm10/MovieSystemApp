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
        public IActionResult Index()
        {
            return View(_context.Actors.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
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
        public IActionResult AddMovie(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor == null) return NotFound();

            ViewBag.ActorId = id;
            return View();
        }

        [HttpPost]
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
