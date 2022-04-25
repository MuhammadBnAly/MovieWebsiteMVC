using MovieWebsiteMVC.Models;
using MovieWebsiteMVC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieWebsiteMVC.ViewModels;
using System.Linq;

namespace MovieWebsiteMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        public MoviesController(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }
        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFormViewModel
            {
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            };
            return View(viewModel);
        }
    }
}
