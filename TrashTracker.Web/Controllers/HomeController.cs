using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly TrashTrackerDbContext _context;

        private String ImageDownloadURL => Url != null
            ? Url.Action(action: "Image", controller: "Trashes",
                values: new { id = "0" }, protocol: Request.Scheme)![0..^2]
            : "";

        public HomeController(TrashTrackerDbContext context)
        {
            _context = context;
        }

        [HttpGet("OnMap")]
        public ActionResult<TrashMap> GetPointsOnMap()
        {
            var points = TrashMap.Create(_context.Trashes.AsNoTracking());

            var json = Serializer.Serialize(points);

            return Ok(json);
        }

        [HttpGet("OnMapDetails/{id}")]
        public async Task<ActionResult<TrashMapDetails>> GetPointByIdOnMapDetailsAsync(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash == null)
                return NotFound();

            return Ok(Serializer.Serialize(TrashMapDetails.Create(trash, ImageDownloadURL)));
        }

        [HttpGet("TrashCsv")]
        public async Task<FileResult> GetTrashCsvAsync(Int32? count)
        {
            var trashes = await _context.Trashes
                .Take(count ?? _context.Trashes.Count())
                .Select(TrashCsv.Projection)
                .ToListAsync();

            using (StringWriter writer = new())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(trashes);
                return File(new System.Text.UTF8Encoding().GetBytes(writer.ToString()!), "text/csv");
            }
        }

        public IActionResult Index(Int32? lat, Int32? lon)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Error
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
