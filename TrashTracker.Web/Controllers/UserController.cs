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
        /// <summary>
        /// <inheritdoc cref="IAuthorizationService"/>
        /// </summary>
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// <inheritdoc cref="TrashTrackerDbContext"/>
        /// </summary>
        private readonly TrashTrackerDbContext _context;

        /// <summary>
        /// <inheritdoc cref="SignInManager{TrashTrackerUser}"/>
        /// </summary>
        private readonly SignInManager<TrashTrackerUser> _signInManager;

        /// <summary>
        /// <inheritdoc cref="UserManager{TrashTrackerUser}"/>
        /// </summary>
        private readonly UserManager<TrashTrackerUser> _userManager;

        /// <summary>
        /// The base of the images' download URL,
        /// defined by <see cref="ImageAsync"/>'s routing.
        /// </summary>
        private String ImageURL => Url != null
            ? Url.Action(action: "Image", controller: "User",
                values: new { id = "0" }, protocol: Request.Scheme)![0..^2]
            : "";

        /// <summary>
        /// Creates an instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="authorizationService">
        /// Reference to an authorization service to check policy based permissions for users.
        /// </param>
        /// <param name="context">Reference to the database's context to access data with.</param>
        /// <param name="signInManager">Reference to a sign-in manager for users.</param>
        /// <param name="userManager">
        /// Reference to a user manager for managing them in the database.
        /// </param>
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

        /// <summary>
        /// Endpoint method to get the login form's view.
        /// </summary>
        /// <param name="returnUrl">The return URL to return to after posting the form.</param>
        /// <returns><inheritdoc cref="Controller.View"/></returns>
        [HttpGet]
        public IActionResult Login(String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Endpoint method to post a login form,
        /// making the user to log in to a matching account (if any).
        /// </summary>
        /// <param name="login"><inheritdoc cref="UserLogin" path="/summary"/></param>
        /// <param name="returnUrl">The return URL to return to after posting the form.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// if the model's state is valid, then containing a <see cref="RedirectResult"/>
        /// with the <paramref name="returnUrl"/> as its URL,
        /// otherwise containing a <see cref="ViewResult"/>
        /// with the <see cref="TrashEdit"/> object sent as its model.
        /// </returns>
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

        /// <summary>
        /// Endpoint method to get the registration form's view.
        /// </summary>
        /// <param name="returnUrl">The return URL to return to after posting the form.</param>
        /// <returns><inheritdoc cref="Controller.View"/></returns>
        [HttpGet]
        public IActionResult Register(String? returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Endpoint method to post a registration form.
        /// </summary>
        /// <param name="register"><inheritdoc cref="UserRegister" path="/summary"/></param>
        /// <param name="returnUrl">The return URL to return to after posting the form.</param>
        /// <returns><inheritdoc cref="Controller.View"/></returns>
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

        /// <summary>
        /// Endpoint method to post a logout form, making the user to log out.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="RedirectResult"/>
        /// defined by <see cref="HomeController.Index"/> as its URL.
        /// </returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion

        #region Queries

        /// <summary>
        /// Endpoint method that returns the view of <see cref="TrashTrackerUser"/> object's list,
        /// given how many and which page should be viewed.
        /// </summary>
        /// <param name="searchString">
        /// The <see cref="String"/> that should be searched by in usernames and e-mail addresses.
        /// </param>
        /// <param name="currentFilter">
        /// The current <see cref="String"/> that was used to searched by in usernames and notes.
        /// </param>
        /// <param name="pageNumber">Number of page to be viewed (defaults to 1).</param>
        /// <param name="pageSize">Size of page to be viewed (defaults to 100).</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>
        /// with the <see cref="PaginatedList{Trash}"/> object
        /// containing its relevant <see cref="Trash"/> objects as its model.
        /// </returns>
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

        /// <summary>
        /// Endpoint method
        /// that returns the view of <see cref="TrashTrackerUser"/> object's details.
        /// </summary>
        /// <param name="userName">
        /// The username of the <see cref="TrashTrackerUser"/> of which details should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>
        /// with a <see cref="UserDetails"/> object with the user's details as its model.
        /// </returns>
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

        /// <summary>
        /// Endpoint method to get a user modification form's view,
        /// only available for non-admin users, if editing their own account.
        /// </summary>
        /// <param name="userName">
        /// The username of the <see cref="TrashTrackerUser"/> of which details should be modified.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>,
        /// with a <see cref="TrashEdit"/> object containing the user's details as its model.
        /// </returns>
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

        /// <summary>
        /// Endpoint method to post a user modification's form, modifying it
        /// if user by its username is found and the submitted model's state is valid.
        /// </summary>
        /// <param name="userName">
        /// The username of the <see cref="TrashTrackerUser"/> to modify.
        /// </param>
        /// <param name="userEdit">
        /// The details of the user to edit the one referred by its username.
        /// </param>
        /// <param name="previousPage">The return URL to return to after posting the form.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// if the model's state is valid, then a containing <see cref="RedirectResult"/>
        /// with the <paramref name="previousPage"/> as its URL,
        /// otherwise containing a <see cref="ViewResult"/>
        /// with the <see cref="TrashEdit"/> object sent as its model.
        /// </returns>
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

        /// <summary>
        /// Endpoint method to get a user deletion form's view.
        /// </summary>
        /// <param name="userName">
        /// The username of the <see cref="TrashTrackerUser"> to delete.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>
        /// containing a <see cref="TrashTrackerUser"/> object as its model.
        /// </returns>
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

        /// <summary>
        /// Endpoint method to post a user deletion's form.
        /// </summary>
        /// <param name="userName">
        /// The username of the <see cref="TrashTrackerUser"/> of which should be deleted.
        /// </param>
        /// <param name="previousPage">The return URL to return to after posting the form.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="RedirectResult"/>
        /// with the <paramref name="previousPage"/> as its URL.
        /// </returns>
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

        /// <summary>
        /// Gets the profile picture with the given id.
        /// </summary>
        /// <param name="id"> The id of the profile picture. </param>
        /// <response code="200">The profile picture was returned successfully</response>
        /// <response code="404">The trash profile picture was not found.</response>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="FileContentResult"/> with the profile picture's file.
        /// </returns>
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
