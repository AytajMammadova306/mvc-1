using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = await _context.Categories.AnyAsync(c=>c.Name==category.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(Category.Name), $"{category.Name} Category already exists");
                return View();
            }


            category.CreatedAt = DateTime.Now;

            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Category existed = _context.Categories.FirstOrDefault(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }

            return View(existed);
        }
    }
}
