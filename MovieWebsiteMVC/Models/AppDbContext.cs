
using Microsoft.EntityFrameworkCore;
using MovieWebsiteMVC.Models.Data;

namespace MovieWebsiteMVC.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Anime> Animes { get; set; }

    }
}
