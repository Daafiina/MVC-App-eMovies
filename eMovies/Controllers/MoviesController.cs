using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eMovies.Data;
using eMovies.Models;
using eMovies.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace eMovies.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service = service;
        }

        [AllowAnonymous]

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var allMovies = await _service.GetAllAsync(n=>n.Cinema);
            return View(allMovies);
        }
        public async Task<IActionResult>Filter(string searchString)
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);

            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredResult=allMovies.Where(n=>n.Name.Contains(searchString)|| n.Description.Contains
                (searchString )).ToList();
                return View("Index", filteredResult);
            }

            return View("Index", allMovies);
        }

        // GET: Movies/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            return View(movieDetail);
        }
        //GET: Movies/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var movieDetails=await _service.GetMovieByIdAsync(id);
            if (movieDetails == null) return View("NotFound");

            var response = new NewMovieVM()
            {
                Id = movieDetails.Id,
                Name = movieDetails.Name,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                StartDate= movieDetails.StartDate,
                EndDate= movieDetails.EndDate,
                ImageURL = movieDetails.ImageURL,
                MovieCategory = movieDetails.MovieCategory,
                CinemaId = movieDetails.CinemaId,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList(),

            };

            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie)
        {
            if (id != movie.Id) return View("NotFound");
            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
                return View(movie);

            }
            await _service.UpdateMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }
        // GET: Movies/Create
        public async Task<IActionResult> Create()
        {
            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            return View();

        }
        [HttpPost]
        public async Task<IActionResult>Create(NewMovieVM movie)
        {
            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
                return View(movie);

            }
            await _service.AddNewMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }


        /*
                // GET: Movies/Details/5
                public async Task<IActionResult> Details(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var movie = await _context.Movies
                        .Include(m => m.Cinema)
                        .Include(m => m.Producer)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (movie == null)
                    {
                        return NotFound();
                    }

                    return View(movie);
                }

                // GET: Movies/Create
                public IActionResult Create()
                {
                    ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Id");
                    ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id");
                    return View();
                }

                // POST: Movies/Create
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageURL,StartDate,EndDate,MovieCategory,CinemaId,ProducerId")] Movie movie)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(movie);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Id", movie.CinemaId);
                    ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", movie.ProducerId);
                    return View(movie);
                }

                // GET: Movies/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var movie = await _context.Movies.FindAsync(id);
                    if (movie == null)
                    {
                        return NotFound();
                    }
                    ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Id", movie.CinemaId);
                    ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", movie.ProducerId);
                    return View(movie);
                }

                // POST: Movies/Edit/5
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageURL,StartDate,EndDate,MovieCategory,CinemaId,ProducerId")] Movie movie)
                {
                    if (id != movie.Id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(movie);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!MovieExists(movie.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Id", movie.CinemaId);
                    ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", movie.ProducerId);
                    return View(movie);
                }

                // GET: Movies/Delete/5
                public async Task<IActionResult> Delete(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var movie = await _context.Movies
                        .Include(m => m.Cinema)
                        .Include(m => m.Producer)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (movie == null)
                    {
                        return NotFound();
                    }

                    return View(movie);
                }

                // POST: Movies/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    var movie = await _context.Movies.FindAsync(id);
                    _context.Movies.Remove(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                private bool MovieExists(int id)
                {
                    return _context.Movies.Any(e => e.Id == id);
                }*/
    }
}
