using inclass.Models;
using Microsoft.EntityFrameworkCore;

namespace inclass.DAL
{
    public class AppDbContext:DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> option):base(option)
        { }

        public DbSet<Slide> Slides { get; set; }
    }
}
