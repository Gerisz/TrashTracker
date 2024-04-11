using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Data.Models.DTOs.Query;
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
            String currentFilter, Int32? pageNumber, Int32? pageSize)
        {
            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["currentFilter"] = searchString;
            var count = _context.Trashes.Count();
            /*var trashesFromStart = _context.Trashes
                .OrderBy(x => x.Id)
                .Take((id - 1) * size + size)
                .OrderByDescending(x => x.Id);
            var trashesFromEnd = trashesFromStart.Count() != count
                ? trashesFromStart.Take(size)
                : trashesFromStart.Take(count % ((id - 1) * size));*/
            var trashes = _context.Trashes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Include(t => t.User);
            if (!searchString.IsNullOrEmpty())
                trashes = (IIncludableQueryable<Trash, TrashTrackerUser?>)trashes
                    .Where(t => !t.Note.IsNullOrEmpty() && t.Note!.Contains(searchString!));
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            return View(await PaginatedList<Trash>
                .CreateAsync(trashes, pageNumber ?? 1, pageSize ?? 100));
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

        private bool TrashExists(int id)
        {
            return _context.Trashes.Any(e => e.Id == id);
        }
    }
}
