using System.ComponentModel;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class UserIndex
    {
        public String Id { get; set; } = null!;

        [DisplayName("Felhasználónév")]
        public String UserName { get; set; } = null!;

        [DisplayName("E-mail cím")]
        public String Email { get; set; } = null!;

        [DisplayName("Regisztrálás ideje")]
        public DateTime RegistrationTime { get; set; }

        public static Expression<Func<TrashTrackerUser, UserIndex>> Projection { get; }
            = user => new UserIndex()
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                RegistrationTime = user.RegistrationTime
            };
    }
}
