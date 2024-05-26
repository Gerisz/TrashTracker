using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Geometries;
using System;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.In;
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
            Assert.Equal(2, model?.Count);
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
            // Act
            var searchString = "uniqueNote";
            var pageSize = 100;
            var model = ((await _controller.Index(searchString, "", 1, pageSize, false))
                as ViewResult)?.Model
                as PaginatedList<Trash>;

            // Assert
            Assert.DoesNotContain(Status.Cleaned, model!.Select(t => t.Status));
            Assert.Contains(searchString, model!.Select(t => t.Note)!);
        }

        [Fact]
        public async Task DetailsTest()
        {
            // Act
            var trash = await _context.Trashes.FirstOrDefaultAsync();
            var model = ((await _controller.Details(trash!.Id))
                as ViewResult)?.Model
                as TrashDetails;

            // Assert
            Assert.Equal(TrashDetails.Create(trash!, ""), model);
        }

        [Fact]
        public async Task DetailsWithWrongIdTest()
        {
            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(await _controller.Details(null));
            Assert.IsAssignableFrom<NotFoundResult>(await _controller.Details(-1));
        }

        [Fact]
        public void CreateGetTest()
        {
            // Act
            var model = (_controller.Create() as ViewResult)!.Model as TrashFromUser;

            // Assert
            Assert.IsAssignableFrom<TrashFromUser>(model);
            Assert.Equal(new TrashFromUser(), model);
        }

        [Fact]
        public async Task CreatePostTest()
        {
            // Act
            var count = _context.Trashes.Count() + 1;
            var trash = DbInitializer.CreateRandomTrash(new Random(),
                NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(WGS_SRID));
            var model = await _controller.Create(new TrashFromUser(trash), "");

            // Assert
            Assert.IsAssignableFrom<RedirectToActionResult>(model);
            Assert.Equal(count, _context.Trashes.Count());
            Assert.NotEmpty(_context.Trashes.Where(t => t.Location == trash.Location));
        }

        [Fact]
        public async Task EditGetTest()
        {
            // Act
            var trash = await _context.Trashes.FirstOrDefaultAsync();
            var model = (await _controller.Edit(trash!.Id) as ViewResult)!.Model as TrashEdit;

            // Assert
            Assert.IsAssignableFrom<TrashEdit>(model);
            Assert.Equal(new TrashEdit(trash), model);
        }

        [Fact]
        public async Task EditPostTest()
        {
            // Act
            var count = _context.Trashes.Count();
            var trash = await _context.Trashes.FirstOrDefaultAsync();
            var newTrash = DbInitializer.CreateRandomTrash(new Random(),
                NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(WGS_SRID));
            newTrash.Id = trash!.Id;

            // Assert
            Assert.IsAssignableFrom<RedirectToActionResult>
                (await _controller.Edit(trash!.Id, new TrashEdit(newTrash)));
            Assert.Equal(count, _context.Trashes.Count());
            Assert.NotNull(_context.Trashes.Where(t => t.Location == newTrash.Location));
        }

        [Fact]
        public async Task DeleteGetTest()
        {
            // Act
            var trash = await _context.Trashes.FirstOrDefaultAsync();
            var model = (await _controller.Delete(trash!.Id) as ViewResult)!.Model as Trash;

            // Assert
            Assert.IsAssignableFrom<Trash>(model);
            Assert.Equal(trash, model);
        }

        [Fact]
        public async Task DeleteGetWrongIdTest()
        {
            // Act
            var count = _context.Trashes.Count();
            var model = await _controller.Delete(-1);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(model);
            Assert.Equal(count, _context.Trashes.Count());
        }

        [Fact]
        public async Task DeletePostTest()
        {
            // Act
            var count = _context.Trashes.Count() - 1;
            var trash = await _context.Trashes.FirstOrDefaultAsync();
            var model = await _controller.DeleteConfirmed(trash.Id);

            // Assert
            Assert.IsAssignableFrom<RedirectToActionResult>(model);
            Assert.Null(await _context.Trashes.FindAsync(trash.Id));
            Assert.Equal(count, _context.Trashes.Count());
        }

        [Fact]
        public async Task DeletePostWrongIdTest()
        {
            // Act
            var count = _context.Trashes.Count();
            var model = await _controller.DeleteConfirmed(-1);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(model);
            Assert.Equal(count, _context.Trashes.Count());
        }

        private const Int32 WGS_SRID = 4326;
    }
}
