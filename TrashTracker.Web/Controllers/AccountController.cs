using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<TrashTrackerUser> _userManager;
        private readonly SignInManager<TrashTrackerUser> _signInManager;

        public AccountController(UserManager<TrashTrackerUser> userManager,
            SignInManager<TrashTrackerUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login vm, String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(vm.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", "Sikertelen bejelentkezés!");
                    return View(vm);
                }

                var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);

                ModelState.AddModelError("", "Sikertelen bejelentkezés!");
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Register(String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register vm, String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new TrashTrackerUser { UserName = vm.UserName };
                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError("", "Sikertelen regisztráció!");
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(TrashesController.Index), nameof(TrashesController));
        }

        private IActionResult RedirectToLocal(String? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(TrashesController.Index), nameof(TrashesController));
        }
    }
}
