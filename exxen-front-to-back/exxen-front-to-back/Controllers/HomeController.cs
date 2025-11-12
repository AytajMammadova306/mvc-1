using Microsoft.AspNetCore.Mvc;

namespace exxen_front_to_back.Controllers
{
    public class HomeController:Controller 
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
