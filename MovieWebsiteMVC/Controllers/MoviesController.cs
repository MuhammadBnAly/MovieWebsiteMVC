using MovieWebsiteMVC.Models;
using MovieWebsiteMVC.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace MovieWebsiteMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        public MoviesController(AppDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
