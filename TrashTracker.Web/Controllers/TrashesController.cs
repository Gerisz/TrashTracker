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
    public class TrashesController : Controller
    {
        private readonly TrashTrackerDbContext _context;

        private String ImageURL => Url != null
            ? Url.Action(action: "Image", controller: "Trashes",
                values: new { id = "0" }, protocol: Request.Scheme)![0..^2]
            : "";

        public TrashesController(TrashTrackerDbContext context)
        {
            _context = context;
        }

        // GET: Trashes/5
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

        // GET: Trashes/Details/5
        public async Task<IActionResult> Details(Int32? id)
        {
            if (id == null)
                return NotFound();

            var trash = await _context.Trashes
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trash == null)
                return NotFound();

            if(Request != null)
                ViewData["previousPage"] = Request.Headers.Referer.ToString();

            return View(TrashDetails
                .Create(trash, ImageURL));
        }

        // GET: Trashes/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["previousPage"] = Request.Headers.Referer.ToString();
            return View(new TrashFromUser());
        }

        // POST: Trashes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrashFromUser trashFromUser, String previousPage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Trash(trashFromUser,
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!));
                await _context.SaveChangesAsync();

                return RedirectToLocal(previousPage);
            }
            ViewData["previousPage"] = previousPage;
            return View(trashFromUser);
        }

        // GET: Trashes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash == null)
                return NotFound();

            ViewData["previousPage"] = Request.Headers.Referer.ToString();
            return View(new TrashEdit(trash));
        }

        // POST: Trashes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32 id, TrashEdit trashEdit, String previousPage)
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

        // GET: Trashes/Delete/5
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

            ViewData["previousPage"] = Request.Headers.Referer.ToString();
            return View(trash);
        }

        // POST: Trashes/Delete/5
        [Authorize(Policy = "Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 id, String previousPage)
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash != null)
                _context.Trashes.Remove(trash);

            await _context.SaveChangesAsync();
            return RedirectToLocal(previousPage);
        }

        /// <summary>
        /// Gets the place image with the given id.
        /// </summary>
        /// <param name="id"> The id of the image. </param>
        /// <response code="200">The image was returned successfully</response>
        /// <response code="404">The place image was not found.</response>
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
            if (Url.IsLocalUrl(returnUrl))
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
