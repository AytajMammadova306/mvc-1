using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.Utilities.Extentions;
using Pronia_self.ViewModels;
using System.Threading.Tasks;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

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
            List<GetSlideVM> slideVMs = await _context.Slides
                .AsNoTracking()
                .Select(s=>new GetSlideVM 
                {
                    Title = s.Title,
                    Order = s.Order,
                    Id = s.Id,
                    Image = s.Image,
                })
                .ToListAsync();


            return View(slideVMs);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slideVM.Photo.ValidateType("image/")){
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File type is incorrect");
                return View();
            }
            if (!slideVM.Photo.ValidateSize(Utilities.Enums.FileSize.MB,2))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File size is incorrext");
                return View();
            }


            bool result = await _context.Slides.AnyAsync(s => s.Order == slideVM.Order);
            if (result)
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Order), $"{slideVM.Order} Order already exists");
                return View();
            }



            string filename = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
            Slide slide = new Slide
            {
                Title = slideVM.Title,
                Subtitle = slideVM.Subtitle,
                Order = slideVM.Order,
                Description = slideVM.Description,
                Image = filename,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };





            _context.Add(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Slide existed =await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }

            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Title = existed.Title,
                Subtitle = existed.Subtitle,
                Order = existed.Order,
                Description = existed.Description,
                Image = existed.Image
            };

            return View(slideVM);
        }
        [HttpPost]

        public async Task<IActionResult> Update(int? id, UpdateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View(slideVM);
            }
            if(slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File type is incorrect");
                    return View();
                }
                if (!slideVM.Photo.ValidateSize(Utilities.Enums.FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File size is incorrext");
                    return View();
                }
            }

            bool result = await _context.Slides.AnyAsync(s => s.Order == slideVM.Order && s.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateSlideVM.Order), $"Slide with {slideVM.Order} order already exists");
                return View(slideVM);
            }

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if(existed is null)
            {
                return NotFound();  
            }

            if(slideVM.Photo is not null)
            {
                string fileName =await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image = fileName;
            }

            existed.Order = slideVM.Order;
            existed.Description = slideVM.Description;
            existed.Subtitle = slideVM.Subtitle;
            existed.Title = slideVM.Title;
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            _context.Remove(existed);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
