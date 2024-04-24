using System.ComponentModel;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Utils;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class UserDetails
    {
        [DisplayName("Felhasználónév")]
        public String UserName { get; set; } = null!;
        [DisplayName("E-mail cím")]
        public String Email { get; set; } = null!;
        public Boolean EmailConfirmed { get; set; }
        public String Role { get; set; } = null!;
        public Uri? Image { get; set; }
        public PaginatedList<Trash>? Trashes { get; set; }

        public static Expression<Func<TrashTrackerUser, UserDetails>> Projection
            (String role, String imageUrlBase, PaginatedList<Trash>? trashes)
            => user => new UserDetails()
            {
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                Role = role,
                Image = user.Image != null
                    ? new Uri($"{imageUrlBase}/{user.Image.Id}")
                    : null,
                Trashes = trashes
            };

        public static UserDetails Create(TrashTrackerUser user, String role,
            String imageUrlBase, PaginatedList<Trash>? trashes)
            => Projection(role, imageUrlBase, trashes).Compile().Invoke(user);
    }
}
