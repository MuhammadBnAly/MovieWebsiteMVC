using MovieWebsiteMVC.Models;
using MovieWebsiteMVC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieWebsiteMVC.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using AnimeListMVC.Consts;
using NToastNotify;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace MovieWebsiteMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IToastNotification _toastNotification;

        private Consts constObj = new Consts();
        
        public MoviesController(AppDbContext context, IToastNotification toastNotification)
        {
            this._context = context;
            this._toastNotification = toastNotification;
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

            // check if image is jpg / png
            //var allowedExtensions = new List<string> { ".jpg", ".png" };
            if (! constObj.allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .JPG , .PNG images are allowed.");
                return View("AnimeForm", model);
            }

            // check the size of the image : OneMegaByte = 1 MB = 1048576 B
            // show error msg
            if (poster.Length > constObj.OneMegaByte)
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

            // notification
            _toastNotification.AddSuccessToastMessage("Anime Created Successfully");

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnimeFormViewModel model)
        {
            // check if the user select a category or not
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                return View("AnimeForm", model);
            }

            // if Id is correct 
            var anime = await _context.Animes.SingleOrDefaultAsync(n => n.Id == model.Id);

            if (anime == null)
                return NotFound();

            // edit poster
            var files = Request.Form.Files;
           
            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                // if image has error = it sent to model (null)
                // it will refresh the same page (AnimeForm) with the same image 
                // with the error
                model.Poster = dataStream.ToArray();

                // check if image is jpg / png
                if (!constObj.allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .JPG , .PNG images are allowed.");
                    return View("AnimeForm", model);
                }

                // check the size of the image : OneMegaByte = 1 MB = 1048576 B
                // show error msg
                if (poster.Length > constObj.OneMegaByte)
                {
                    model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster Can't be more than 1 MB.");
                    return View("AnimeForm", model);
                }
                anime.Poster = model.Poster;
            }
            
            // mapping
            anime.Title = model.Title;
            anime.Year = model.Year;
            anime.Rate = model.Rate;
            anime.CategoryId = model.CategoryId;
            anime.StoreLine = model.StoreLine;

            await _context.SaveChangesAsync();

            // notification
            _toastNotification.AddSuccessToastMessage("Anime updated Successfully");

            return RedirectToAction(nameof(Index));

        }


    }
}
