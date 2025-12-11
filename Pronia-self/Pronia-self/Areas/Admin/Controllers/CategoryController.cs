using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.ViewModels;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var categoryVMs = await _context.Categories
            .Select(c=>new GetCategoryVM
            {
                Name = c.Name,
                Id = c.Id,
                ProductCount= _context.Products.Where(p=>p.CategoryId == c.Id).Count(),
            }).ToListAsync();
            return View(categoryVMs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (categoryVM.Name.Length == 0)
            {
                ModelState.AddModelError(nameof(UpdateCategoryVM), "Category Name should be at least one character");
            }
            bool result = await _context.Categories.AnyAsync(c=>c.Name== categoryVM.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(CreateCategoryVM.Name), $"{categoryVM.Name} Category already exists");
                return View();
            }
            _context.Add(new Category
            {
                Name = categoryVM.Name,
                CreatedAt = DateTime.Now,
                IsDeleted=false,
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Category existed =await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            UpdateCategoryVM categoryVM = new UpdateCategoryVM
            {
                Name= existed.Name,
            };
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }
            if (categoryVM.Name.Length == 0)
            {
                ModelState.AddModelError(nameof(UpdateCategoryVM), "Category Name should be at least one character");
            }
            bool result = await _context.Categories.AnyAsync(c => c.Name == categoryVM.Name&&c.Id!=id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateCategoryVM.Name), $"{categoryVM.Name} Category already exists");
                return View();
            }
            Category? existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            existed.Name= categoryVM.Name;
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
