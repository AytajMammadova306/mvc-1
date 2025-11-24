using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.ViewModels;

namespace Pronia_self.Controllers
{
    public class ShopController:Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id < 1)
            {
                return BadRequest();
            }


            Product? product =await _context.Products
                .Include(p=>p.ProductImages.OrderByDescending(pi=>pi.IsPrimary))
                .Include(p=>p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            List<Product> relatedProducts=await _context.Products
                .Where(p=>p.Category==product.Category&&p.Id!=product.Id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .ToListAsync();

            DetailsVM detailsVM = new DetailsVM
            {
                Product = product,
                RelatedProduct=relatedProducts
            };
            return View(detailsVM);
        }
    }
}
