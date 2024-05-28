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
    /// <summary>
    /// A <see cref="Controller"/> <see cref="class"/>,
    /// containing endpoint behaviors provided for the homepage and its map.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Reference to the database's context to access data with.
        /// </summary>
        private readonly TrashTrackerDbContext _context;

        /// <summary>
        /// The base of the images' download URL,
        /// defined by <see cref="TrashesController.ImageAsync"/>'s routing.
        /// </summary>
        private String ImageURL => Url != null
            ? Url.Action(action: "Image", controller: "Trashes",
                values: new { id = "0" }, protocol: Request.Scheme)![0..^2]
            : "";

        /// <summary>
        /// Creates an instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="context">Reference to the database's context to access data with.</param>
        public HomeController(TrashTrackerDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint method that returns all the points in a .geojson format,
        /// to be shown on the map.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the .geojson formatted string.
        /// </returns>
        [HttpGet("OnMap")]
        public ActionResult<TrashMap> GetPointsOnMap()
        {
            var points = TrashMap.Create(_context.Trashes.AsNoTracking());

            var json = Serializer.Serialize(points);

            return Ok(json);
        }

        /// <summary>
        /// Endpoint method that returns a point's images and its notes by its id,
        /// to be shown on the map, after clicking on one.
        /// </summary>
        /// <param name="id">The id of the point of which details should be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing an <see cref="ActionResult"/> containing the point's images and its notes.
        /// </returns>
        [HttpGet("OnMapDetails/{id}")]
        public async Task<ActionResult<TrashMapDetails>> GetPointByIdOnMapDetailsAsync(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);

            if (trash == null)
                return NotFound();

            return Ok(Serializer.Serialize(TrashMapDetails.Create(trash, ImageURL)));
        }

        /// <summary>
        /// Endpoint method that returns all the points in a .csv format,
        /// to be downloaded as a file.
        /// </summary>
        /// <param name="limit">
        /// Optional limit (how much points to contain in the file), useful when debugging.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents an asynchronous operation,
        /// containing a <see cref="FileContentResult"/> with the .csv file.
        /// </returns>
        [HttpGet("TrashCsv")]
        public async Task<FileResult> GetTrashCsvAsync(Int32? limit)
        {
            var trashes = await _context.Trashes
                .Take(limit ?? _context.Trashes.Count())
                .Select(TrashCsv.Projection)
                .ToListAsync();

            using (StringWriter writer = new())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(trashes);
                return File(new System.Text.UTF8Encoding().GetBytes(writer.ToString()!), "text/csv");
            }
        }

        /// <summary>
        /// Endpoint method that returns the homepage's view.
        /// </summary>
        /// <param name="lat">Optional latitude coordinate, to zoom into (if given).</param>
        /// <param name="lon">Optional longitude coordinate, to zoom into (if given).</param>
        /// <returns><inheritdoc cref="Controller.View"/></returns>
        #pragma warning disable IDE0060 // Parameters not used here, but used in client-side scripts
        public IActionResult Index(Int32? lat, Int32? lon)
        #pragma warning restore IDE0060
        {
            return View();
        }

        /// <summary>
        /// Endpoint method that returns an error's view.
        /// </summary>
        /// <returns><inheritdoc cref="Controller.View"/></returns>
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
