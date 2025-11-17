using Microsoft.AspNetCore.Mvc;

namespace inclass.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login_register()
        {
            return View();
        }
    }
}
