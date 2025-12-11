using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.ViewModels;

namespace Pronia_self.Controllers
{
    public class HomeController:Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Slides =await _context.Slides
                .OrderBy(s => s.Order)
                .Take(3)
                .ToListAsync(),
                Products = await _context.Products
                .OrderBy(p=>p.CreatedAt)
                .Take(8)
                .Include(p => p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .ToListAsync()
            };
            return View(homeVM);


            //List<Slide> slides = new List<Slide>
            //{
            //    new Slide
            //    {
            //        Title="Basliq 1",
            //        Subtitle="Subtitle 1",
            //        Describtion="Description Description Description",
            //        CreatedAt=DateTime.Now,
            //        IsDeleted=false,
            //        Image="1-2-524x617.png",
            //        Order=3
            //    },
            //    new Slide
            //    {
            //        Title="Basliq 2",
            //        Subtitle="Subtitle 2",
            //        Describtion="Description Description Description",
            //        CreatedAt=DateTime.Now,
            //        IsDeleted=false,
            //        Image="1-1-524x617.png",
            //        Order=1
            //    },
            //    new Slide
            //    {
            //        Title="Basliq 3",
            //        Subtitle="Subtitle 3",
            //        Describtion="Description Description Description",
            //        CreatedAt=DateTime.Now,
            //        IsDeleted=false,
            //        Image="1-2-570x633.jpg",
            //        Order=2
            //    }
            //};
            //_context.Slides.AddRange(slides);
            //_context.SaveChanges();


        }

        //public IActionResult Test()
        //{
        //    HttpContext.Response.Cookies.Append("Name","AYtaj",new CookieOptions
        //    {
        //        MaxAge=TimeSpan.FromSeconds(7)
        //    });
        //    return Content("OK");
        //}
        //public IActionResult GetCookie()
        //{
        //    return Content(HttpContext.Request.Cookies["Name"]);
        //}


    }
}
