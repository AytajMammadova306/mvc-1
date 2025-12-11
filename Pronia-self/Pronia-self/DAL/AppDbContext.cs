using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pronia_self.Models;

namespace Pronia_self.DAL
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> option):base(option)
        { }

        public DbSet<Slide> Slides { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags{ get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductSize> ProdcutSizes { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductTag>().HasKey(pt => new { pt.ProductId, pt.TagId });
            modelBuilder.Entity<ProductColor>().HasKey(pc => new { pc.ProductId, pc.ColorId });
            modelBuilder.Entity<ProductSize>().HasKey(ps => new { ps.ProductId, ps.SizeId });
            modelBuilder.Entity<Setting>().HasKey(s =>s.Key);

        }

    }
}
