using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using System.Threading.Tasks;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.AsNoTracking().ToListAsync();
            return View(slides);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {


            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}

            if (!slide.Photo.ContentType.Contains("image/")){
                ModelState.AddModelError(nameof(Slide.Photo), "File type is incorrect");
                return View();
            }
            if (slide.Photo.Length > 2*1024 * 1024)
            {
                ModelState.AddModelError(nameof(Slide.Photo), "File size is incorrext");
                return View();
            }


            bool result = await _context.Slides.AnyAsync(s => s.Order == slide.Order);
            if (result)
            {
                ModelState.AddModelError(nameof(Slide.Order), $"{slide.Order} Order already exists");
                return View();
            }
            string fileName = string.Concat(Guid.NewGuid(), Path.GetExtension(slide.Photo.FileName));
            string path = Path.Combine(_env.WebRootPath,"assets","images","website-images" ,fileName);
            FileStream stream = new FileStream(path,FileMode.Create);

            slide.Photo.CopyToAsync(stream);
            slide.Image = fileName;



            slide.CreatedAt = DateTime.Now;

            _context.Add(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Slide existed = _context.Slides.FirstOrDefault(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }

            return View(existed);
        }
        [HttpPost]

        public async Task<IActionResult> Update(int? id, Slide slide)
        {
            if (!ModelState.IsValid)
            {
                return View(slide);
            }
            bool result = await _context.Slides.AnyAsync(s => s.Order == slide.Order && s.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Slide.Order), $"Slide with {slide.Order} order already exists");
                return View(slide);
            }
            slide.CreatedAt = DateTime.Now;

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if(existed is null)
            {
                return NotFound();  
            }
            existed.Order = slide.Order;
            existed.Describtion = slide.Describtion;
            existed.Subtitle = slide.Subtitle;
            existed.Title = slide.Title;
            existed.Image = slide.Image;
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }
    }
}
