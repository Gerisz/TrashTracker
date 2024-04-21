using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<TrashTrackerUser> _userManager;
        private readonly SignInManager<TrashTrackerUser> _signInManager;

        public UserController(UserManager<TrashTrackerUser> userManager,
            SignInManager<TrashTrackerUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Authorization

        [HttpGet]
        public IActionResult Login(String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login, String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);

                user ??= await _userManager.FindByEmailAsync(login.UserName);

                if (user == null)
                {
                    ModelState.AddModelError("", "Sikertelen bejelentkezés!");
                    return View(login);
                }

                var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);

                ModelState.AddModelError("", "Sikertelen bejelentkezés!");
            }

            return View(login);
        }

        [HttpGet]
        public IActionResult Register(String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register register, String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new TrashTrackerUser(register);
                var result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError("", "Sikertelen regisztráció!");
            }

            return View(register);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion

        #region Queries

        [HttpGet]
        public async Task<ActionResult<UserDetails>> Details(String userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();

            return View(UserDetails.Create(user, ""));
        }

        #endregion

        private IActionResult RedirectToLocal(String? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(TrashesController.Index), "Trashes");
        }
    }
}
