using Microsoft.AspNetCore.Mvc;

namespace Pronia_self.Controllers
{
    public class HomeController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
