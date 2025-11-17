using Microsoft.AspNetCore.Mvc;

namespace inclass.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
