using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.Utilities.Enums;
using Pronia_self.Utilities.Extentions;
using Pronia_self.ViewModels;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

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
                Categories = await _context.Categories.ToListAsync(),
                Tags=await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes=await _context.Sizes.ToListAsync(),

            };

            return View(productVm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Tags= await _context.Tags.ToListAsync();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.Colors= await _context.Colors.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if (!productVM.PrimaryImage.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.PrimaryImage),"File type is incorrect");
                return View(productVM);
            }
            if (!productVM.SecondaryImage.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.SecondaryImage), "File type is incorrect");
                return View(productVM);
            }
            if (!productVM.PrimaryImage.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateProductVM.PrimaryImage), "File size is incorrect");
                return View(productVM);
            }
            if (!productVM.SecondaryImage.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateProductVM.SecondaryImage), "File size is incorrect");
                return View(productVM);
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category doesnot exist(custom)");
                return View(productVM);
            }
            if(productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }
            productVM.TagIds = productVM.TagIds.Distinct().ToList() ;
            if (productVM.TagIds.Any(ti => !productVM.Tags.Exists(t => t.Id == ti)))
            {
                ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
                return View(productVM);
            }
            productVM.ColorIds = productVM.ColorIds.Distinct().ToList();
            if (productVM.ColorIds.Any(ti => !productVM.Colors.Exists(t => t.Id == ti)))
            {
                ModelState.AddModelError(nameof(CreateProductVM.ColorIds), "Colors are wrong");
                return View(productVM);
            }
            productVM.SizeIds = productVM.SizeIds.Distinct().ToList();
            if (productVM.SizeIds.Any(ti => !productVM.Sizes.Exists(t => t.Id == ti)))
            {
                ModelState.AddModelError(nameof(CreateProductVM.SizeIds), "Colors are wrong");
                return View(productVM);
            }
            bool resultName = await _context.Products.AnyAsync(p => p.Name == productVM.Name);
            if (resultName)
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), "Product Name already exists");
                return View(productVM);
            }
            //List<ProductImage> productImages = new List<ProductImage>
            //{
            //    new ProductImage
            //    {
            //        Image
            //    }
            //};

            Product product = new()
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                Price = productVM.Price.Value,
                Description = productVM.Description,
                CategoryId = productVM.CategoryId.Value,
                CreatedAt = DateTime.Now,
                ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList(),
                ProductSizes = productVM.SizeIds.Select(sId => new ProductSize { SizeId = sId }).ToList(),
                ProductColors = productVM.ColorIds.Select(cId => new ProductColor { ColorId = cId }).ToList(),
                ProductImages = new List<ProductImage>{
                    new ProductImage
                    {
                        Image=await productVM.PrimaryImage.CreateFileAsync(_env.WebRootPath,"assets","images","website-images"),
                        IsPrimary= true,
                        CreatedAt= DateTime.Now,
                    },
                    new ProductImage
                    {
                        Image=await productVM.SecondaryImage.CreateFileAsync(_env.WebRootPath,"assets","images","website-images"),
                        IsPrimary= false,
                        CreatedAt= DateTime.Now,
                    }
                }
            };
            string message = string.Empty;
            if(productVM.AdditionalPhotos is not null)
            {
                foreach (IFormFile file in productVM.AdditionalPhotos)
                {
                    if (!file.ValidateType("image/"))
                    {
                        message += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} type is incorrect\r\n</div>";
                        continue;
                    }
                    if (!file.ValidateSize(FileSize.MB, 2))
                    {
                        message += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} size is incorrect\r\n</div>";
                        continue;
                    }
                    product.ProductImages.Add(new()
                    {
                        Image = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        IsPrimary = null,
                        CreatedAt = DateTime.Now
                    }
                    );
                }
            }
            TempData["ImageWarning"] = message;

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
            Product? existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p=>p.ProductTags)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == id);
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
                Categories = await _context.Categories.ToListAsync(),
                Tags= await _context.Tags.ToListAsync(),
                Sizes= await _context.Sizes.ToListAsync(),
                Colors= await _context.Colors.ToListAsync(),
                ColorIds=existed.ProductColors.Select(pt=>pt.ColorId).ToList(),
                SizeIds=existed.ProductSizes.Select(pt=>pt.SizeId).ToList(),
                TagIds=existed.ProductTags.Select(pt=>pt.TagId).ToList(),
                ProductImages = existed.ProductImages,
            };

            return View(productVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            Product? existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if(productVM.PrimaryImage is not null)
            {
                if (!productVM.PrimaryImage.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.PrimaryImage), "File type is incorrect");
                    return View(productVM);
                }
                if (!productVM.PrimaryImage.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.PrimaryImage), "File size is incorrect");
                    return View(productVM);
                }
            }
            if(productVM.SecondaryImage is not null)
            {
                if (!productVM.SecondaryImage.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SecondaryImage), "File type is incorrect");
                    return View(productVM);
                }
                if (!productVM.SecondaryImage.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SecondaryImage), "File size is incorrect");
                    return View(productVM);
                }
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateProductVM), "Category doesnot exist(custom)");
                return View(productVM);
            }
            if (productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }
            productVM.TagIds = productVM.TagIds.Distinct().ToList();
            if (productVM.TagIds.Any(ti => !productVM.Tags.Exists(t => t.Id == ti)))
            {
                ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
                return View(productVM);
            }

            bool resultName = await _context.Categories.AnyAsync(p => p.Name == productVM.Name&& p.Id!=id);
            if (resultName)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.Name), "Product Name already exists");
                return View(productVM);
            }

            if(productVM.PrimaryImage is not null)
            {
                string mainFileName =await productVM.PrimaryImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "web-images");
                ProductImage existedmain = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                existedmain.Image.DeleteFile(_env.WebRootPath,"assets", "images", "web-images");
                existed.ProductImages.Add(new ProductImage
                {
                    Image = mainFileName,
                    CreatedAt = DateTime.Now,
                    IsPrimary = true
                });
                existed.ProductImages.Remove(existedmain);
            }
            if(productVM.SecondaryImage is not null)
            {
                string SecondaryFileName =await productVM.SecondaryImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "web-images");
                ProductImage existedSecondary = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                existedSecondary.Image.DeleteFile(_env.WebRootPath,"assets", "images", "web-images");
                existed.ProductImages.Add(new ProductImage
                {
                    Image = SecondaryFileName,
                    CreatedAt = DateTime.Now,
                    IsPrimary = false
                });
                existed.ProductImages.Remove(existedSecondary);
            }

            _context.ProductTags
                .RemoveRange(existed.ProductTags
                    .Where(pt=>!productVM.TagIds
                        .Exists(tId=>tId==pt.TagId))
                    .ToList());
            _context.ProductTags
                .AddRange(productVM.TagIds
                    .Where(ti=>!existed.ProductTags
                        .Exists(pt=>pt.TagId==ti))
                    .Select(tid=>new ProductTag {TagId=tid,ProductId=existed.Id})
                    .ToList());
            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.SKU = productVM.SKU;
            existed.Price=productVM.Price.Value;
            existed.CategoryId=productVM.CategoryId.Value;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details (int? id)
        {
           
            GetDetailsVM? detailsVM = await _context.Products
                .Include(p=>p.ProductImages)
                .ThenInclude(pi=>pi.Product)
                .Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors)
                .ThenInclude(ps => ps.Color)
                .Select(p => new GetDetailsVM
                { 
                Id=p.Id,
                Name = p.Name,
                Description = p.Description,
                SKU = p.SKU,
                Price = p.Price,
                Category = p.Category.Name,
                ProductImages = p.ProductImages,
                Tags=p.ProductTags
                    .Select(pt=>pt.Tag.Name)
                    .ToList(),
                Sizes=p.ProductSizes
                    .Select(ps=>ps.Size.Name)
                    .ToList(),
                Colors=p.ProductColors
                    .Select(pc=>pc.Color.Name)
                    .ToList()
                })
                .FirstOrDefaultAsync(p => p.Id == id);
  
            return View(detailsVM);
        }
    }
}
