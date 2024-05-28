using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Controllers
{
    /// <summary>
    /// A <see cref="Controller"/> <see cref="class"/>,
    /// containing endpoint behaviors provided for views associated with <see cref="Trash"> objects.
    /// </summary>
    public class TrashesController : Controller
    {
        /// <summary>
        /// Reference to the database's context to access data with.
        /// </summary>
        private readonly TrashTrackerDbContext _context;

        /// <summary>
        /// The base of the images' download URL,
        /// defined by <see cref="ImageAsync"/>'s routing.
        /// </summary>
        private String ImageURL => Url != null
            ? Url.Action(action: "Image", controller: "Trashes",
                values: new { id = "0" }, protocol: Request.Scheme)![0..^2]
            : "";

        /// <summary>
        /// Creates an instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="context">
        /// Reference to the database's context to access data with.
        /// </param>
        public TrashesController(TrashTrackerDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint method that returns the view of <see cref="Trash"/> object's list,
        /// given how many and which page should be viewed.
        /// </summary>
        /// <param name="searchString">
        /// The <see cref="String"/> that should be searched by in usernames and notes.
        /// </param>
        /// <param name="currentFilter">
        /// The current <see cref="String"/> that was used to searched by in usernames and notes.
        /// </param>
        /// <param name="pageNumber">Number of page to be viewed (defaults to 1).</param>
        /// <param name="pageSize">Size of page to be viewed (defaults to 100).</param>
        /// <param name="showCleaned">
        /// Logical value if cleaned points should be included in the list.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>
        /// containing the <see cref="PaginatedList{Trash}"/> object
        /// with its relevant <see cref="Trash"/> objects as its model.
        /// </returns>
        public async Task<IActionResult> Index(String? searchString,
            String currentFilter, Int32? pageNumber, Int32? pageSize, Boolean? showCleaned)
        {
            // query every trash from database
            var trashes = _context.Trashes
                .Include(t => t.User)
                .AsNoTracking();

            // if no new serach string is given
            if (searchString.IsNullOrEmpty())
                // then let the previous serach term be the filtering string
                searchString = currentFilter;
            // and make it the current filter as well
            ViewData["currentFilter"] = searchString;

            if (!searchString.IsNullOrEmpty())
                // filter by either username or note
                trashes = trashes
                    .Where(t => !(t.Note == null || t.Note == "") && t.Note.Contains(searchString!)
                        || !(t.User == null || t.User!.UserName == null || t.User.UserName == "")
                            && t.User.UserName.Contains(searchString!));

            // showCleaned into ViewData for correct switch position in form
            ViewData["showCleaned"] = showCleaned ?? false;
            // if we dont want to show cleaned
            if (!showCleaned ?? false)
                // then filter them
                trashes = trashes.Where(t => t.Status != Status.Cleaned);

            // order points by freshly updated
            trashes = trashes.OrderByDescending(x => x.UpdateTime);

            // paginate them
            var paginatedTrashes = await PaginatedList<Trash>
                .CreateAsync(trashes, pageNumber ?? 1, pageSize ?? 100);

            // if requested page has no points
            if (paginatedTrashes.Count() <= 0)
                // then return the first page
                return View(await PaginatedList<Trash>
                    .CreateAsync(trashes, 1, pageSize ?? 100));
            // else just return the requested page with its points
            return View(paginatedTrashes);
        }

        /// <summary>
        /// Endpoint method
        /// that returns the view of a <see cref="Trash"/> object's details page by id.
        /// </summary>
        /// <param name="id">
        /// The id of the <see cref="Trash"/> of which details should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>
        /// containing a <see cref="TrashDetails"/> object with the point's details as its model.
        /// </returns>
        public async Task<IActionResult> Details(Int32? id)
        {
            if (id == null)
                return NotFound();

            var trash = await _context.Trashes
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trash == null)
                return NotFound();

            if (Request != null)
                ViewData["previousPage"] = Request.Headers.Referer.ToString();

            return View(TrashDetails
                .Create(trash, ImageURL));
        }

        /// <summary>
        /// Endpoint method to get a point creation form's view.
        /// </summary>
        /// <returns>
        /// A <see cref="ViewResult"/>
        /// containing a default <see cref="TrashFromUser"/> object as its model.
        /// </returns>
        [Authorize]
        public IActionResult Create()
        {
            if (Request != null)
                ViewData["previousPage"] = Request.Headers.Referer.ToString();

            return View(new TrashFromUser());
        }

        /// <summary>
        /// Endpoint method to post a point creation's form,
        /// creating a point, if the submitted model's state is valid.
        /// </summary>
        /// <param name="trashFromUser">
        /// The details of the point to create the <see cref="Trash"/> object by.
        /// </param>
        /// <param name="previousPage">The return URL to return to after posting the form.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing if the model's state is valid,
        /// then a <see cref="RedirectResult"/> with the <paramref name="previousPage"/> as its URL,
        /// otherwise a <see cref="ViewResult"/>
        /// containing the <see cref="TrashFromUser"/> object sent as its model.
        /// </returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrashFromUser trashFromUser,
            String previousPage = "")
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(new Trash(trashFromUser,
                    User != null
                        ? User.FindFirstValue(ClaimTypes.NameIdentifier)!
                        : ""));
                await _context.SaveChangesAsync();

                return RedirectToLocal(previousPage);
            }
            ViewData["previousPage"] = previousPage;
            return View(trashFromUser);
        }

        /// <summary>
        /// Endpoint method to get a point modification form's view.
        /// </summary>
        /// <param name="id">
        /// The id of the <see cref="Trash"/> of which details should be modified.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>,
        /// containing a <see cref="TrashEdit"/> object with the trash's details as its model.
        /// </returns>
        [Authorize]
        public async Task<IActionResult> Edit(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash == null)
                return NotFound();

            if (Request != null)
                ViewData["previousPage"] = Request.Headers.Referer.ToString();

            return View(new TrashEdit(trash));
        }

        /// <summary>
        /// Endpoint method to post a point modification's form, modifying it
        /// if trash by its id is found and the submitted model's state is valid.
        /// </summary>
        /// <param name="id">
        /// The id of the <see cref="Trash"/> of which details should be modified.
        /// </param>
        /// <param name="trashEdit">
        /// The details of the trash to edit the one referred by its id.
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
        public async Task<IActionResult> Edit(Int32 id, TrashEdit trashEdit,
            String previousPage = "")
        {
            if (id != trashEdit.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var trash = await _context.Trashes.FindAsync(id);

                    if (trash == null)
                        return NotFound();

                    trash.Update(new Trash(trashEdit, trash.UserId!));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrashExists(trashEdit.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToLocal(previousPage);
            }

            return View(trashEdit);
        }

        /// <summary>
        /// Endpoint method to get a point deletion's form's view.
        /// </summary>
        /// <param name="id">The id of the <see cref="Trash"> to delete.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="ViewResult"/>
        /// containing a <see cref="Trash"/> object as its model.
        /// </returns>
        [Authorize(Policy = "Moderator")]
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (id == null)
                return NotFound();

            var trash = await _context.Trashes
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trash == null)
                return NotFound();

            if (Request != null)
                ViewData["previousPage"] = Request.Headers.Referer.ToString();

            return View(trash);
        }

        /// <summary>
        /// Endpoint method to post a point deletion's form.
        /// </summary>
        /// <param name="id">The id of the <see cref="Trash"> to delete.</param>
        /// <param name="previousPage">The return URL to return to after posting the form.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="RedirectResult"/>
        /// with the <paramref name="previousPage"/> as its URL.
        /// </returns>
        [Authorize(Policy = "Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 id, String previousPage = "")
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash == null)
                return NotFound();

            _context.Trashes.Remove(trash!);
            await _context.SaveChangesAsync();
            return RedirectToLocal(previousPage);
        }

        /// <summary>
        /// Gets the trash image with the given id.
        /// </summary>
        /// <param name="id"> The id of the image. </param>
        /// <response code="200">The image was returned successfully</response>
        /// <response code="404">The trash image was not found.</response>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="FileContentResult"/> with the image's file.
        /// </returns>
        [HttpGet("[controller]/Image/{id}")]
        public async Task<IActionResult> ImageAsync(Int32 id)
        {
            var image = await _context.TrashImages.FindAsync(id);

            if (image == null || image.Image == null)
                return NotFound();

            var imageStream = new MemoryStream(image.Image);

            return File(imageStream, image.ContentType!);
        }

        private IActionResult RedirectToLocal(String? returnUrl)
        {
            if (Url != null && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(Index), "Trashes");
        }

        private bool TrashExists(Int32 id)
        {
            return _context.Trashes.Any(e => e.Id == id);
        }
    }
}
