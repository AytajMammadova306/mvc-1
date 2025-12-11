using Azure.Core;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.Services.Interfaces;
using Pronia_self.ViewModels;
using System.Security.Claims;

namespace Pronia_self.Services.Implementations
{
    public class LayoutService:ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly HttpContext? _httpcontext;

        //private readonly IHttpContextAccessor _accessor;

        public LayoutService(AppDbContext context,IHttpContextAccessor accessor)
        {
            _context = context;
            _httpcontext = accessor.HttpContext;
            //_accessor= accessor;
        }

        public async Task<Dictionary<string, string>> GetSettingAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return settings;
        }

        public async Task<BasketVM> GetBasketVMAsync()
        {
            BasketVM basketVM = new BasketVM()
            {
                BasketItemVMs = new List<BasketItemVM>()
            };

            if (_httpcontext.User.Identity.IsAuthenticated)
            {
                basketVM.BasketItemVMs = await _context.BasketItems
                    .Where(bi => bi.AppUserId == _httpcontext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi => new BasketItemVM()
                    {
                        ProductId = bi.ProductId,
                        Count = bi.Count,
                        Name = bi.Product.Name,
                        Price = bi.Product.Price,
                        Image = bi.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                        SubTotal = bi.Count * bi.Product.Price
                    })
                    .ToListAsync();
                basketVM.BasketItemVMs.ForEach(b => basketVM.Total += b.SubTotal);
            }

            else
            {
                string json = _httpcontext.Request.Cookies["Basket"];
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



            return basketVM;
        }

    }
}
