using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.Utilities.Enums;
using Pronia_self.ViewModels;

namespace Pronia_self.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _roleManager=roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(userVM);
            }
            AppUser user = new AppUser
            {
                UserName = userVM.UserName,
                Email = userVM.Email,
                Name = userVM.Name,
                Surname = userVM.Surname,
            };

            IdentityResult result= await _userManager.CreateAsync(user,userVM.Password);
            if (!result.Succeeded)
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());

            await _signInManager.SignInAsync(user, isPersistent: false);
            if (returnUrl is not null)
            {
                return (Redirect(returnUrl));
            }

            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM userVM,string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = await _userManager.Users.
                FirstOrDefaultAsync(u => u.UserName == userVM.UserNameOrEmail || u.Email == userVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"Username, Email or Password is incorrect");
                return View();
            }
            var result=await _signInManager.PasswordSignInAsync(user,userVM.Password,userVM.IsPersistnet,true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is blocked please try later");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username, Email or Password is incorrect");
                return View();
            }
            Response.Cookies.Delete("Basket");
            if (returnUrl is not null)
            {
                return (Redirect(returnUrl));
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut(string? returnUrl)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl is not null)
            {
                return (Redirect(returnUrl));
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> CreateRoles()
        {
            
            foreach(UserRole role in Enum.GetValues(typeof(UserRole))){
                if(!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    IdentityRole identityRole = new()
                    {
                        Name = role.ToString()
                    };
                    await _roleManager.CreateAsync(identityRole);
                }
                

            }
            return RedirectToAction("Index", "Home");
        }
    }
}
