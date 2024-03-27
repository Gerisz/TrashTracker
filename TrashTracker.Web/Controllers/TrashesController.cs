using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Tables;

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
        public async Task<IActionResult> Index(Int32 id, Int32 size)
        {
            var count = _context.Trashes.Count();
            var trashesFromStart = _context.Trashes
                .OrderBy(x => x.Id)
                .Take((id - 1) * size + size)
                .OrderByDescending(x => x.Id);
            var trashesFromEnd = trashesFromStart.Count() != count
                ? trashesFromStart.Take(size)
                : trashesFromStart.Take(count % ((id - 1) * size));
            var trashes = trashesFromEnd.OrderBy(x => x.Id)
                .Include(t => t.User)
                .AsEnumerable();
            ViewData["id"] = id;
            ViewData["size"] = size;
            ViewData["max"] = (Int32)Math.Ceiling(_context.Trashes.Count() / (Double)size);
            return View(trashes);
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
            return View(new TrashFromUser());
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

        private bool TrashExists(int id)
        {
            return _context.Trashes.Any(e => e.Id == id);
        }
    }
}
