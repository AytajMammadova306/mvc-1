using Microsoft.AspNetCore.Mvc;

namespace inclass.Controllers
{
    public class ShopeController : Controller
    {
        public IActionResult Wishlist()
        {
            return View();
        }
    }
}
