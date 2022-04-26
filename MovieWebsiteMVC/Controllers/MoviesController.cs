using MovieWebsiteMVC.Models;
using MovieWebsiteMVC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieWebsiteMVC.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MovieWebsiteMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        public MoviesController(AppDbContext context)
        {
            this._context = context;
        }

        //Sorting 
        public async Task<IActionResult> sortByName()
        {
            var result = await _context.Animes.OrderBy(n => n.Title).ToListAsync();
            return View(nameof(Index) , result);
        }
        public async Task<IActionResult> sortByRate()
        {
            var result = await _context.Animes.OrderByDescending(n => n.Rate).ToListAsync();
            return View(nameof(Index), result);
        }
        public async Task<IActionResult> sortByOlder()
        {
            var result = await _context.Animes.OrderBy(n => n.Year).ToListAsync();
            return View(nameof(Index), result);

        }
        public async Task<IActionResult> sortByNewer()
        {
            var result = await _context.Animes.OrderByDescending(n => n.Year).ToListAsync();
            return View(nameof(Index), result);

        }

        //
        public async Task<IActionResult> Index()
        {
            var animes = await _context.Animes.ToListAsync();
            return View(animes);
        }
        public async Task<IActionResult> Create()
        {
            var viewModel = new AnimeFormViewModel
            {
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            };
            return View("AnimeForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnimeFormViewModel model)
        {
            // check if the user select a category or not
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                return View("AnimeForm", model);
            }

            // check if user select a poster or not
            // show error msg to user
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Select Anime Poster");
                return View("AnimeForm", model);
            }

            // check if image jpg/ png or not
            // show error msg to user
            var poster = files.FirstOrDefault();
            var allowedExtensions = new List<string> { ".jpg", ".png" };
            if (! allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .JPG , .PNG images are allowed.");
                return View("AnimeForm", model);
            }

            // check the size of the image : 1 MB = 1048576 B
            // show error msg
            if (poster.Length > 1048576)
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster Can't be more than 1 MB.");
                return View("AnimeForm", model);
            }

            // using stream
            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);

            // Mapping ( AnimeFormViewModel ==> Anime )
            var anime = new Anime
            {
                Title = model.Title,
                CategoryId = model.CategoryId,
                Year = model.Year,
                Rate = model.Rate,
                StoreLine = model.StoreLine,
                Poster = dataStream.ToArray()
            };

            // save data to database
            await _context.Animes.AddAsync(anime);
            await _context.SaveChangesAsync();
            
            //return RedirectToAction("Index");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var anime = await _context.Animes.SingleOrDefaultAsync(n => n.Id == id);
            
            if (anime == null)
                return NotFound();

            var viewModel = new AnimeFormViewModel
            {
                Id = anime.Id,
                Title = anime.Title,
                Year = anime.Year,
                Rate =anime.Rate,
                CategoryId=anime.CategoryId,
                StoreLine=anime.StoreLine,
                Poster=anime.Poster,
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            };
            return View("AnimeForm", viewModel);
        }

    }
}
