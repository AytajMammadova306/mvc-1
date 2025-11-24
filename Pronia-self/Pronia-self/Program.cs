using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;

namespace Pronia_self
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Defualt"));
            });
            var app = builder.Build();
            app.UseStaticFiles();

            app.MapControllerRoute(
                "defualt",
                "{area:exists}/{controller=home}/{action=index}/{id?}");

            app.MapControllerRoute(
                "defualt",
                "{controller=home}/{action=index}/{id?}");

            app.Run();
        }
    }
}
