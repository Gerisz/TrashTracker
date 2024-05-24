using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Geometries;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Data.Models.Enums;
using System.Runtime.InteropServices;

namespace TrashTracker.Data.Models
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(TrashTrackerDbContext context)
        {
            // do not migrate when testing
            if (context.Database.IsRelational())
                context.Database.Migrate();
            // instead seed database with a few trashes and save them to test with
            else
            {
                await SeedTrashesAsync(context, 10);
            }

        }

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

        public static async Task SeedTrashesAsync(TrashTrackerDbContext context, Int32 count)
        {
            ICollection<Trash> trashes = [];

            for (Int32 i = 0; i < count; i++)
                trashes.Add(CreateTrash());

            await context.Trashes.AddRangeAsync(trashes);
            await context.SaveChangesAsync();
        }

        private static Trash CreateTrash()
        {
            Random random = new();
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

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
