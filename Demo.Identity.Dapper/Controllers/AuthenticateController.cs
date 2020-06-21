using System.Threading.Tasks;
using Demo.Identity.Dapper.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Identity.Dapper.Controllers
{
    public class AuthenticateController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticateController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser model)
        {
            if (!ModelState.IsValid)
                return View();

            model.Image = "This is test custom property that got insert up to database!";
            var result = await _userManager.CreateAsync(model, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
                    await _roleManager.CreateAsync(new ApplicationRole(){ Name = "SuperAdmin" });
                if (!await _roleManager.RoleExistsAsync("Guest"))
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = "Guest" });

                if (model.IsAdministrator)
                    await _userManager.AddToRoleAsync(model, "SuperAdmin");
                else
                    await _userManager.AddToRoleAsync(model, "Guest");

                await _signInManager.SignInAsync(model, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return View();
        }

        public async Task<IActionResult> Login()
        {
            // Clear the existing external cookie to ensure a clean login process
            await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUser model)
        {
            if (!ModelState.IsValid) return View();

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            if (result.RequiresTwoFactor)
            {
                ModelState.AddModelError(string.Empty, "Required 2 Factor Authenticated!");
                return View();
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "This user is looked out!");
                return View();
            }

            ModelState.AddModelError(string.Empty, "UserName or Password is not correct!");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}