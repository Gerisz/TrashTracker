using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Geometries;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Data.Models.Enums;

namespace TrashTracker.Data.Models
{
    /// <summary>
    /// A <see langword="static"/> <see langword="class"/> containing several functions,
    /// to help initialize the database with some default values.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Initializes <paramref name="context"/> by migrating it when it's a relational database,
        /// otherwise seed it with some data (e. g. to test with).
        /// </summary>
        /// <param name="context">A <see cref="TrashTrackerDbContext"/> to initalize.</param>
        /// <returns>A <see cref="Task"> that represents the asynchronous operation.</returns>
        public static async Task InitializeAsync(TrashTrackerDbContext context)
        {
            if (context.Database.IsRelational())
                context.Database.Migrate();
            else
                await SeedTrashesAsync(context, 10);
        }

        /// <summary>
        /// Seeds the <paramref name="userManager"/> with default users.
        /// </summary>
        /// <param name="userManager">
        /// A <see cref="UserManager{TrashTrackerUser}"/> to seed users with.
        /// </param>
        /// <returns>A <see cref="Task"> that represents the asynchronous operation.</returns>
        public static async Task SeedUsersAsync(UserManager<TrashTrackerUser> userManager)
        {
            // for every role defined in DefaultRoles.cs
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                var user = await userManager.FindByNameAsync($"{role}");
                // if user with said role doesn't exist
                if (user == null)
                {
                    // then create a default user for that role
                    user = new TrashTrackerUser()
                    {
                        UserName = $"{role}".ToLower(),
                        Email = $"{role}@example.com".ToLower(),
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, $"{role}{role}1");
                    await userManager.AddToRoleAsync(user, $"{role}");
                }
                else
                {
                    user.UserName = $"{role}".ToLower();
                    user.Email = $"{role}@example.com".ToLower();
                    user.EmailConfirmed = true;

                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(user, $"{role}");
                }
            }
        }

        /// <summary>
        /// Seeds the <paramref name="roleManager"/> with default users.
        /// </summary>
        /// <param name="roleManager"></param>
        /// <returns>A <see cref="Task"> that represents the asynchronous operation.</returns>
        public static async Task SeedRolesAsync(RoleManager<TrashTrackerIdentityRole> roleManager)
        {
            // for every role defined in DefaultRoles.cs
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                // if there's no said role
                if (!await roleManager.RoleExistsAsync($"{role}"))
                    // then create that role
                    await roleManager.CreateAsync(new TrashTrackerIdentityRole($"{role}"));
            }
        }

        /// <summary>
        /// Seeds the <paramref name="context"/> with <paramref name="count"/>
        /// amount of <see cref="Trash"/>.
        /// </summary>
        /// <param name="context">
        /// A <see cref="TrashTrackerDbContext"/> to seed <paramref name="context"/> into.
        /// </param>
        /// <param name="count">
        /// How many random <see cref="Trash"/> to seed into <paramref name="context"/>
        /// (will contain some extra default trashes, regardless the value of this parameter).
        /// </param>
        /// <returns>A <see cref="Task"> that represents the asynchronous operation.</returns>
        public static async Task SeedTrashesAsync(TrashTrackerDbContext context, Int32 count)
        {
            ICollection<Trash> trashes = [];
            Random random = new();

            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            for (Int32 i = 0; i < count; i++)
                trashes.Add(CreateRandomTrash(random, gf));

            trashes.Add(new Trash
            {
                Location = (Point)GeometryFixer.Fix(gf.CreatePoint(
                new Coordinate()
                {
                    X = Math.Round(random.NextDouble() + 47, 6),
                    Y = Math.Round(random.NextDouble() * 2 + 19, 6)
                })),
                Country = Country.Hungary,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.UtcNow,
                UpdateNeeded = false,
                Note = "uniqueNote",
                Accessibilities = 0,
                Size = Size.Wheelbarrow,
                Status = Status.Cleaned,
                Types = 0,
                Images = []
            });

            trashes.Add(new Trash
            {
                Location = (Point)GeometryFixer.Fix(gf.CreatePoint(
                new Coordinate()
                {
                    X = Math.Round(random.NextDouble() + 47, 6),
                    Y = Math.Round(random.NextDouble() * 2 + 19, 6)
                })),
                Country = Country.Hungary,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.UtcNow,
                UpdateNeeded = false,
                Note = "uniqueNote",
                Accessibilities = 0,
                Size = Size.Wheelbarrow,
                Status = Status.StillHere,
                Types = 0,
                Images = []
            });

            await context.Trashes.AddRangeAsync(trashes);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a <see cref="Trash"/> with random values and returns it.
        /// </summary>
        /// <param name="random">
        /// A <see cref="Random"/> to generate the location and filter properties with.
        /// </param>
        /// <param name="gf">
        /// A <see cref="GeometryFactory"/> to create the location's <see cref="Point"/> with.
        /// </param>
        /// <returns>The <see cref="Trash"/> created by this function.</returns>
        public static Trash CreateRandomTrash(Random random, GeometryFactory gf)
        {
            Accessibility accessibilities = new();
            TrashType types = new();
            Enum.GetValues<Accessibility>().ToList()
                .ForEach(v => accessibilities |= (random.Next(2) == 0 ? v : 0));
            Enum.GetValues<TrashType>().ToList()
                .ForEach(v => types |= (random.Next(2) == 0 ? v : 0));

            return new Trash()
            {
                Location = (Point)GeometryFixer.Fix(gf.CreatePoint(
                new Coordinate()
                {
                    X = Math.Round(random.NextDouble() + 47, 6),
                    Y = Math.Round(random.NextDouble() * 2 + 19, 6)
                })),
                Country = Country.Hungary,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.UtcNow,
                UpdateNeeded = false,
                Note = "Note",
                Accessibilities = accessibilities,
                Size = (Size)random.Next(Enum.GetValues<Size>().Length + 1),
                Status = (Status)random.Next(Enum.GetValues<Status>().Length + 1),
                Types = types,
                Images = []
            };
        }
    }
}
