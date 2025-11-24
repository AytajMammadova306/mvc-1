using inclass.DAL;
using inclass.Models;
using inclass.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inclass.Controllers
{
    public class HomeController:Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Slides = _context.Slides.OrderBy(s => s.Order).Take(3).ToList(),
                Products = _context.Products
                .OrderBy(p=>p.CreatedAt)
                .Take(8)
                .Include(p => p.ProductImages)
                .ToList()
            };

            return View(homeVM);
            //List<Slide> slides = new List<Slide>
            //{
            //    new Slide
            //    {
            //        Title="Title 1",
            //        CreatedAt = DateTime.Now,
            //        Subtitle="Subtitle 1",
            //        Price=50,
            //        Order=1,
            //        IsDeleted=false,
            //        Image="home-slider-1-ai.png"
            //    },
            //    new Slide
            //    {
            //        Title="Title 2",
            //        CreatedAt = DateTime.Now,
            //        Subtitle="Subtitle 2",
            //        Price=60,
            //        Order=2,
            //        IsDeleted=false,
            //        Image="home-slider-2-ai.png"
            //    },
            //    new Slide
            //    {
            //        Title="Title 3",
            //        CreatedAt = DateTime.Now,
            //        Subtitle="Subtitle 3",
            //        Price=70,
            //        Order=3,
            //        IsDeleted=false,
            //        Image="home-slider-3-ai.png"
            //    }
            //};

            //_context.AddRange(slides);
            //_context.SaveChanges();


        }
        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
