﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Controllers
{
    public class TrashesController : Controller
    {
        private readonly TrashTrackerDbContext _context;

        public TrashesController(TrashTrackerDbContext context)
        {
            _context = context;
        }

        // GET: Trashes/5
        public async Task<IActionResult> Index(String? sortOrder, String? searchString,
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trash = await _context.Trashes
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trash == null)
            {
                return NotFound();
            }

            return View(trash);
        }

        // GET: Trashes/Create
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrashFromUser trashFromUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Trash(trashFromUser));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trashFromUser);
        }

        // GET: Trashes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trash = await _context.Trashes.FindAsync(id);
            if (trash == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", trash.UserId);
            return View(trash);
        }

        // POST: Trashes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrashoutId,UserId,Location,Country,CreateTime,UpdateTime,UpdateNeeded,Note,Status,Size,Types,Accessibilities")] Trash trash)
        {
            if (id != trash.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trash);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrashExists(trash.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", trash.UserId);
            return View(trash);
        }

        // GET: Trashes/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
        [HttpGet("Image/{id}")]
        public async Task<IActionResult> DownloadPlaceImageAsync(int id)
        {
            var image = await _context.TrashImages.FindAsync(id);

            if (image == null || image.Image == null)
                return NotFound();

            var imageStream = new MemoryStream(image.Image);

            return File(imageStream, image.ContentType!);
        }

        private bool TrashExists(int id)
        {
            return _context.Trashes.Any(e => e.Id == id);
        }
    }
}
