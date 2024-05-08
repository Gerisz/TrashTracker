using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly TrashTrackerDbContext _context;
        private readonly SignInManager<TrashTrackerUser> _signInManager;
        private readonly UserManager<TrashTrackerUser> _userManager;

        private string ImageURL => Url
            .Action(action: "Image", controller: "User",
            values: new { id = "0" }, protocol: Request.Scheme)![0..^2];

        public UserController(IAuthorizationService authorizationService,
            TrashTrackerDbContext context,
            SignInManager<TrashTrackerUser> signInManager,
            UserManager<TrashTrackerUser> userManager)
        {
            _authorizationService = authorizationService;
            _context = context;
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
        public async Task<IActionResult> Login(UserLogin login, String? returnUrl = null)
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

                var result = await _signInManager
                    .PasswordSignInAsync(user, login.Password, false, false);

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
        public async Task<IActionResult> Register(UserRegister register, String? returnUrl = null)
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

                ModelState.AddModelError("",
                    String.Join("\n", result.Errors.Select(e => e.Description)));
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

        [Authorize("Admin")]
        [HttpGet]
        public async Task<ActionResult<PaginatedList<UserIndex>>> Index(String? searchString,
            String currentFilter, Int32? pageNumber, Int32? pageSize)
        {
            // query every user from database
            var users = _userManager.Users.AsNoTracking();

            // if no new serach string is given
            if (searchString.IsNullOrEmpty())
                // then let the previous serach term be the filtering string
                searchString = currentFilter;
            // and make it the current filter as well
            ViewData["currentFilter"] = searchString;

            if (!searchString.IsNullOrEmpty())
                // filter by either username or e-mail address
                users = users
                    .Where(u => !u.UserName.IsNullOrEmpty() && u.UserName!.Contains(searchString!)
                        || !u.Email.IsNullOrEmpty() && u.Email!.Contains(searchString!));

            // order users by freshly registered
            var usersOnList = users
                .Select(UserIndex.Projection)
                .OrderByDescending(x => x.RegistrationTime);

            // paginate them
            var paginatedUsersOnList = await PaginatedList<UserIndex>
                .CreateAsync(usersOnList, pageNumber ?? 1, pageSize ?? 100);

            // if requested page has no points
            if (paginatedUsersOnList.Count() <= 0)
                // then return the first page
                return View(await PaginatedList<UserIndex>
                    .CreateAsync(usersOnList, 1, pageSize ?? 100));
            // else just return the requested page with its points
            return View(paginatedUsersOnList);
        }

        [HttpGet]
        public async Task<ActionResult<UserDetails>> Details(String userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();

            return View(UserDetails.Create(user,
                (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "",
                ImageURL, await PaginatedList<Trash>.CreateAsync(
                    _context.Trashes
                        .Include(t => t.User)
                        .Where(t => t.User != null && userName == t.User.UserName),
                    1, 100)));
        }

        // GET: Trashes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(String userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (!IsCurrentUser(user)
                && !(await _authorizationService.AuthorizeAsync(User, "Admin")).Succeeded)
                return Forbid();

            ViewData["previousPage"] = Request.Headers.Referer.ToString();
            try
            {
                return View(new UserEdit(user,
                    Enum.Parse<Roles>((await _userManager.GetRolesAsync(user))[0]), ImageURL));
            }
            catch (ArgumentOutOfRangeException)
            {
                return View(new UserEdit(user, Roles.User, ImageURL));
            }
        }

        // POST: Trashes/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit
            (String userName, UserEdit userEdit, String previousPage)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var conflictingUserByNewUserName = await _userManager
                .FindByNameAsync(userEdit.NewUserName);
            var conflictingUserByEmail = await _userManager
                .FindByEmailAsync(userEdit.Email);

            if (conflictingUserByNewUserName != null && user != conflictingUserByNewUserName)
                ModelState.AddModelError("", "A felhasználónév már foglalt!");
            if (conflictingUserByEmail != null && user != conflictingUserByEmail)
                ModelState.AddModelError("", "Az e-mail cím már foglalt!");

            if (ModelState.IsValid)
            {

                if (user == null)
                    return NotFound();
                if (!IsCurrentUser(user)
                    && !(await _authorizationService.AuthorizeAsync(User, "Admin")).Succeeded)
                    return Forbid();

                if (!IsCurrentUser(user)
                    && (await _authorizationService.AuthorizeAsync(User, "Admin")).Succeeded)
                {
                    var result = await _userManager
                        .RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                    if (!result.Succeeded)
                        return BadRequest(result.Errors);

                    result = await _userManager.AddToRoleAsync(user, $"{userEdit.Role}");

                    if (!result.Succeeded)
                        return BadRequest(result.Errors);
                }

                if (user.UserName != userEdit.NewUserName)
                    previousPage = $"{new Uri(previousPage).Query
                        .Replace(userName, userEdit.NewUserName)}";

                await user.UpdateAsync(_userManager, userEdit);
                await _context.SaveChangesAsync();

                if (IsCurrentUser(user))
                    await _signInManager.SignInAsync(user, false);

                return RedirectToLocal(previousPage);
            }

            return View(userEdit);
        }

        // GET: User/Delete/UserName
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(String userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (IsCurrentUser(user))
                return Forbid();

            ViewData["previousPage"] = Request.Headers.Referer.ToString();
            return View(user);
        }

        // POST: User/Delete/UserName
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(String userName, String previousPage)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (IsCurrentUser(user))
                return Forbid();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToLocal(previousPage);

            return BadRequest(result.Errors);
        }

        [HttpGet("[controller]/Image/{id}")]
        public async Task<IActionResult> ImageAsync(Int32 id)
        {
            var image = await _context.UserImages.FindAsync(id);

            if (image == null)
                return NotFound();

            var imageStream = new MemoryStream(image.Image ?? []);

            return File(imageStream, image.ContentType!);
        }

        #endregion

        private IActionResult RedirectToLocal(String? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(TrashesController.Index), "Trashes");
        }

        private Boolean IsCurrentUser(TrashTrackerUser user)
            => User.FindFirstValue(ClaimTypes.NameIdentifier)! == user.Id;

        private Boolean IsCurrentUserById(String userId)
            => User.FindFirstValue(ClaimTypes.NameIdentifier)! == userId;
    }
}
