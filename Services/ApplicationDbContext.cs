using Microsoft.EntityFrameworkCore;
using MyGagetsWebApp.Models;

namespace MyGagetsWebApp.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
