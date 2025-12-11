using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]

    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
