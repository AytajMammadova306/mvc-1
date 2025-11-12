namespace exxen_front_to_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            app.UseStaticFiles();//onemli, ikisaat bu yaddan cixib diye eziyyet cekilib
            app.MapControllerRoute(
                "defualt",
                "{controller=home}/{action=index}/{id?}");

            app.Run();
        }
    }
}
