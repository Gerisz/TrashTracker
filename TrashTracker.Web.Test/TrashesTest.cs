using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Controllers;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Test
{
    /// <summary>
    /// Test class meant to test MVC endpoints of <see cref="TrashesController"/>.
    /// </summary>
    public class TrashesTest : IDisposable
    {
        private static TrashTrackerDbContext _context = null!;
        private static TrashesController _controller = null!;

        public TrashesTest()
        {
            var options = new DbContextOptionsBuilder<TrashTrackerDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new TrashTrackerDbContext(options);
            Task.Run(() => DbInitializer.InitializeAsync(_context)).Wait();

            _context.ChangeTracker.Clear();

            _controller = new TrashesController(_context);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task IndexNoArgTest()
        {
            // Act
            var pageSize = 100;
            var model = ((await _controller.Index("", "", 1, pageSize, true))
                as ViewResult)?.Model
                as PaginatedList<Trash>;

            // Assert
            Assert.Equal(Math.Min(_context.Trashes.Count(), pageSize), model?.Count);
            Assert.Equal(_context.Trashes.Any(t => t.Status == Status.Cleaned),
                model?.Any(t => t.Status == Status.Cleaned));
        }

        [Fact]
        public async Task IndexSmallPageSizeTest()
        {
            var pageSize = 1;

            for (Int32 i = 1; i < _context.Trashes.Count() + 1; i++)
            {
                // Act
                var model = ((await _controller.Index("", "", i, pageSize, true))
                    as ViewResult)?.Model
                    as PaginatedList<Trash>;

                // Assert
                Assert.Single(model!);
            }
        }

        [Fact]
        public async Task IndexSearchStringTest()
        {
            // Act
            var searchString = "uniqueNote";
            var model = ((await _controller.Index(searchString, "", 1, 100, true))
                as ViewResult)?.Model
                as PaginatedList<Trash>;

            // Assert
            Assert.Contains(searchString, model!.Select(t => t.Note)!);
            Assert.Single(model!);
        }

        [Fact]
        public async Task IndexNoCleanedTest()
        {
            // Act
            var model = ((await _controller.Index("", "", 1, 100, false))
                as ViewResult)?.Model
                as PaginatedList<Trash>;

            // Assert
            Assert.DoesNotContain(Status.Cleaned, model!.Select(t => t.Status));
        }

        [Fact]
        public async Task IndexMultipleConditionsTest()
        {

            var searchString = "uniqueNote";
            var pageSize = 100;
            var model = ((await _controller.Index(searchString, "", 1, pageSize, false))
                as ViewResult)?.Model
                as PaginatedList<Trash>;

            Assert.DoesNotContain(Status.Cleaned, model!.Select(t => t.Status));
            Assert.Contains(searchString, model!.Select(t => t.Note)!);
        }

        [Fact]
        public async Task DetailsTest()
        {
            var trash = await _context.Trashes.FirstOrDefaultAsync();
            var model = ((await _controller.Details(trash.Id))
                as ViewResult)?.Model
                as TrashDetails;

            Assert.Equal(TrashDetails.Create(trash!, ""), model);
        }

        [Fact]
        public async Task DetailsWithWrongIdTest()
        {
            Assert.IsAssignableFrom<NotFoundResult>(await _controller.Details(null));
            Assert.IsAssignableFrom<NotFoundResult>(await _controller.Details(-1));
        }

        [Fact]
        public async Task CreateTest()
        {

        }

        [Fact]
        public async Task EditTest()
        {

        }

        [Fact]
        public async Task DeleteTest()
        {

        }
    }
}
