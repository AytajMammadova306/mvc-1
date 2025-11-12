using Microsoft.AspNetCore.Mvc;

namespace Pronia_self.Controllers
{
    public class ShopController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}
