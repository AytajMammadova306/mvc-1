using inclass.DAL;
using Microsoft.EntityFrameworkCore;

namespace inclass
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt => {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Defualt"));
            });
            var app = builder.Build();
            app.UseStaticFiles();
            app.MapControllerRoute(
                "defualt","{controller=home}/{action=index}/{id?}");

            app.Run();
        }
    }
}
