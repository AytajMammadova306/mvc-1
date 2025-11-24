using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        public SlideController(AppDbContext context)
        {
            _context = context;
        }

        

        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result =await _context.Slides.AnyAsync(s => s.Order== slide.Order);
            if(result)
            {
                ModelState.AddModelError(nameof(Slide.Order),$"{slide.Order} Order already exists");
                return View();
            }


            slide.CreatedAt = DateTime.Now;

            _context.Add(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int? id)
        {
            if(id is null || id < 1)
            {
                return BadRequest();
            }
            Slide existed = _context.Slides.FirstOrDefault(s => s.Id == id);
            if(existed is null)
            {
                return NotFound();
            }

            return View(existed);
        }
    }
}
