using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.ViewModels;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var productVMs = await _context.Products
            .Select(p => new GetAdminProductVM
            {
                Name = p.Name,
                Id = p.Id,
                Image = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                CategoryName = p.Category.Name,
                Price = p.Price,
            })
            .ToListAsync();
            return View(productVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVm = new()
            {
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM), "Category doesnot exist(custom)");
                return View(productVM);
            }

            bool resultName = await _context.Categories.AnyAsync(p => p.Name == productVM.Name);
            if (resultName)
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), "Product Name already exists");
                return View(productVM);
            }

            Product product = new()
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                Price = productVM.Price.Value,
                Description = productVM.Description,
                CategoryId = productVM.CategoryId.Value,
                CreatedAt = DateTime.Now,
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Product? existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null)
            {
                return NotFound();
            }

            UpdateProductVM productVm = new()
            {
                Name = existed.Name,
                SKU = existed.SKU,
                Description = existed.Description,
                CategoryId = existed.CategoryId,
                Price = existed.Price,
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateProductVM), "Category doesnot exist(custom)");
                return View(productVM);
            }

            bool resultName = await _context.Categories.AnyAsync(p => p.Name == productVM.Name&& p.Id!=id);
            if (resultName)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.Name), "Product Name already exists");
                return View(productVM);
            }

            Product? existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.SKU = productVM.SKU;
            existed.Price=productVM.Price.Value;
            existed.CategoryId=productVM.CategoryId.Value;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
