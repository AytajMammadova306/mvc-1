using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pronia_self.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            BasketVM basketVM = new BasketVM()
            {
                BasketItemVMs = new List<BasketItemVM>()
            };

            if (User.Identity.IsAuthenticated)
            {
                basketVM.BasketItemVMs =await _context.BasketItems
                    .Where(bi=>bi.AppUserId==User
                    .FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi=>new BasketItemVM()
                    {
                        ProductId = bi.ProductId,
                        Count=bi.Count,
                        Name=bi.Product.Name,
                        Price=bi.Product.Price,
                        Image=bi.Product.ProductImages.FirstOrDefault(pi=>pi.IsPrimary==true).Image,
                        SubTotal=bi.Count*bi.Product.Price
                    })
                    .ToListAsync();
                basketVM.BasketItemVMs.ForEach(b => basketVM.Total += b.SubTotal);
            }

            else
            {
                string json = Request.Cookies["Basket"];
                List<BasketCookieItemVM> items;
                

                if (json is not null)
                {
                    items = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(json);
                }
                else
                {
                    items = new List<BasketCookieItemVM>();
                }
                List<BasketCookieItemVM> deleted = new();
                foreach (BasketCookieItemVM cookie in items)
                {
                    Product? product = await _context.Products
                        .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                        .FirstOrDefaultAsync(p => cookie.ProductId == p.Id);
                    if (product is not null)
                    {
                        basketVM.BasketItemVMs.Add(new BasketItemVM
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Image = product.ProductImages[0].Image,
                            Count = cookie.Count,
                            SubTotal = cookie.Count * product.Price
                        });

                        basketVM.Total += cookie.Count * product.Price;
                    }
                    else
                    {
                        deleted.Add(cookie);
                    }
                }
                if (deleted.Count != 0)
                {
                    foreach (BasketCookieItemVM cookie in deleted)
                    {
                        items.Remove(cookie);
                    }
                }
            }

            

            return View(basketVM);
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null || id < 0)
            {
                return BadRequest();
            }
            Product product=_context.Products.FirstOrDefault(product => product.Id == id);
            if (product is null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {

                AppUser? user = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem item=user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if(item is null)
                {
                    user.BasketItems.Add(new BasketItem
                    {
                        Count = 1,
                        ProductId=id.Value,
                        CreatedAt= DateTime.Now,

                    });
                }
                else
                {
                    item.Count++;
                }

                await _context.SaveChangesAsync();

            }
            else
            {
                List<BasketCookieItemVM> items;
                string str = Request.Cookies["basket"];
                if (str is null)
                {
                    items = new List<BasketCookieItemVM>();
                    items.Add(new()
                    {
                        ProductId = id.Value,
                        Count = 1
                    });

                }//reduced??
                else
                {
                    items = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(str);
                    BasketCookieItemVM itemVM = items.FirstOrDefault(i => i.ProductId == id.Value);
                    if (itemVM is null)
                    {
                        items.Add(new()
                        {
                            ProductId = id.Value,
                            Count = 1
                        });
                    }
                    else
                    {
                        itemVM.Count++;
                    }

                }
                string json = JsonConvert.SerializeObject(items);
                Response.Cookies.Append("Basket", json);
            }

                
            return RedirectToAction("Index", "Home");
        }
        public IActionResult GetBasket(int? id)
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}
