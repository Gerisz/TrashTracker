using System.ComponentModel;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class UserDetails
    {
        [DisplayName("Felhasználónév")]
        public String UserName { get; set; } = null!;
        [DisplayName("E-mail cím")]
        public String Email { get; set; } = null!;
        public Boolean EmailConfirmed { get; set; }

        public static Expression<Func<TrashTrackerUser, UserDetails>> Projection
            (String imageUrlBase) => user => new UserDetails()
            {
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed
            };

        public static UserDetails Create(TrashTrackerUser user, String imageUrlBase)
            => Projection(imageUrlBase).Compile().Invoke(user);

    }
}
