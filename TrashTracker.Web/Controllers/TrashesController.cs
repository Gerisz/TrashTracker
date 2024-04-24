﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        private string ImageURL => Url
            .Action(action: "Image", controller: "Trashes",
            values: new { id = "0" }, protocol: Request.Scheme)![0..^2];

        public TrashesController(TrashTrackerDbContext context)
        {
            _context = context;
        }

        // GET: Trashes/5
        public async Task<IActionResult> Index(String? searchString,
            String currentFilter, Int32? pageNumber, Int32? pageSize, Boolean? showCleaned)
        {
            if (searchString.IsNullOrEmpty())
                searchString = currentFilter;

            ViewData["currentFilter"] = searchString;
            var count = _context.Trashes.Count();
            var trashes = _context.Trashes
                .AsNoTracking();

            if (!searchString.IsNullOrEmpty())
                trashes = trashes
                    .Where(t => (t.Note != null || t.Note != "") && t.Note!.Contains(searchString!));

            ViewData["showCleaned"] = showCleaned ?? false;
            if (!showCleaned ?? false)
                trashes = trashes.Where(t => t.Status != Status.Cleaned);

            trashes = trashes.OrderByDescending(x => x.UpdateTime)
                .Include(t => t.User);

            var paginatedTrashes = await PaginatedList<Trash>
                .CreateAsync(trashes, pageNumber ?? 1, pageSize ?? 100);
            if (paginatedTrashes.Count() <= 0)
                return View(await PaginatedList<Trash>
                    .CreateAsync(trashes, 1, pageSize ?? 100));
            return View(paginatedTrashes);
        }

        // GET: Trashes/Details/5
        public async Task<ActionResult<TrashDetails>> Details(Int32? id)
        {
            if (id == null)
                return NotFound();

            var trash = await _context.Trashes
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trash == null)
                return NotFound();

            return View(TrashDetails
                .Create(trash, ImageURL, Request.Headers["Referer"].ToString()));
        }

        // GET: Trashes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View(new TrashFromUser()
            {
                PreviousPage = Request.Headers["Referer"].ToString()
            });
        }

        // POST: Trashes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrashFromUser trashFromUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Trash(trashFromUser));
                await _context.SaveChangesAsync();

                return Redirect(trashFromUser.PreviousPage!);
            }
            return View(trashFromUser);
        }

        // GET: Trashes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash == null)
                return NotFound();

            return View(new TrashEdit(trash)
            {
                PreviousPage = Request.Headers["Referer"].ToString()
            });
        }

        // POST: Trashes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32 id, TrashEdit trashEdit)
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

                    _context.Trashes.Entry(trash).CurrentValues
                        .SetValues(new Trash(trashEdit) { Id = trash.Id });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrashExists(trashEdit.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Redirect(trashEdit.PreviousPage!);
            }

            return View(trashEdit);
        }

        // GET: Trashes/Delete/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (id == null)
                return NotFound();

            var trash = await _context.Trashes
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trash == null)
                return NotFound();

            ViewData["Reffer"] = Request.Headers.Referer.ToString();
            return View(trash);
        }

        // POST: Trashes/Delete/5
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);
            if (trash != null)
            {
                _context.Trashes.Remove(trash);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        private bool TrashExists(Int32 id)
        {
            return _context.Trashes.Any(e => e.Id == id);
        }
    }
}
