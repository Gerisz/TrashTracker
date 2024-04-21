using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class UserDetails
    {
        public String UserName { get; set; } = null!;
        public String Email { get; set; } = null!;
        public Boolean EmailConfirmed { get; set; }
        public static Expression<Func<TrashTrackerUser, UserDetails>> Projection
            (String imageUrlBase) => user => new UserDetails()
            {
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed
            };

        public static TrashDetails Create(TrashTrackerUser user, String imageUrlBase)
            => Projection(imageUrlBase).Compile().Invoke(user);

    }
}
