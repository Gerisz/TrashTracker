using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrashTracker.Data.Models.Defaults;

namespace TrashTracker.Data.Models
{
    public class DbInitializer
    {
        public static void Initialize(TrashTrackerDbContext context, String imageDirectory)
        {
            context.Database.Migrate();
        }

        public static async Task SeedUsersAsync(UserManager<TrashTrackerUser> userManager)
        {
            // for every role defined in DefaultRoles.cs
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                var user = await userManager.FindByNameAsync(role.ToString()!);
                // if user with said role doesn't exist
                if (user == null)
                {
                    // then create a default user for that role
                    user = new TrashTrackerUser()
                    {
                        UserName = role.ToString()!,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, role.ToString()!);
                    await userManager.AddToRoleAsync(user, role.ToString()!);
                }
                else
                {
                    user.UserName = role.ToString();
                    user.EmailConfirmed = true;

                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(user, role.ToString()!);
                }
            }
        }

        public static async Task SeedRolesAsync(RoleManager<TrashTrackerIdentityRole> roleManager)
        {
            // for every role defined in DefaultRoles.cs
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                // if there's no said role
                if (!await roleManager.RoleExistsAsync(role.ToString()!))
                    // then create that role
                    await roleManager.CreateAsync(new TrashTrackerIdentityRole(role.ToString()!));
            }
        }
    }
}
