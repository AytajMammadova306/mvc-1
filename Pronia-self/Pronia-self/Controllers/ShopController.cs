using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.Utilities.Enums;
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
        public async Task<IActionResult> Index( string? search, int? categoryId, int key=1, int page=1)
        {

            IQueryable<Product> query =_context.Products;
            if(search is not null)
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower().Trim()));
            }
            if(categoryId is not null)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            switch (key)
            {
                case (int)SortType.Date:
                    query = query.OrderBy(p => p.CreatedAt);
                    break;
                case (int)SortType.Price:
                    query=query.OrderBy(p=>p.Price);
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            int totalCount= await query.CountAsync();
            int totalPage =(int)Math.Ceiling((double)totalCount/2);
            if (page < 1 || page > totalPage)
            {
                return BadRequest();
            }
            query =query.Skip((totalPage - 1) * 2).Take(2);
            ShopVM shopVM = new()
            {
                ProductVMs =await query.Select(p => new GetProductVM()
                {
                    Id = p.Id,
                    Name = p.Name,
                    PrimaryImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                    SecondaryImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false).Image,
                    Price=p.Price
                }).ToListAsync(),

                CategoryVMs=await _context.Categories.Select(c=> new GetCategoryVM()
                {
                    Id = c.Id,
                    Name= c.Name,
                    ProductCount=c.Products.Count()
                }).ToListAsync(),
                Search=search,
                CategoryId=categoryId,
                Key=key,
                TotalPage=totalPage,
                CurrentPage=page
                
            };

            return View(shopVM);
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
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
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
