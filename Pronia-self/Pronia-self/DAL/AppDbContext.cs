using Microsoft.EntityFrameworkCore;
using Pronia_self.Models;

namespace Pronia_self.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option):base(option)
        { }

        public DbSet<Slide> Slides { get; set; }

    }
}
