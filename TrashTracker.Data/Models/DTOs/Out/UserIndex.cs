using System.ComponentModel;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class UserIndex
    {
        /// <summary>
        /// User's id in the database.
        /// </summary>
        public String Id { get; set; } = null!;

        /// <summary>
        /// The user's username.
        /// </summary>
        [DisplayName("Felhasználónév")]
        public String UserName { get; set; } = null!;

        /// <summary>
        /// The user's e-mail address to be contacted at by other users.
        /// </summary>
        [DisplayName("E-mail cím")]
        public String Email { get; set; } = null!;

        /// <summary>
        /// The user's date and time of registration (0000. 00. 00. if it's a default account).
        /// </summary>
        [DisplayName("Regisztrálás ideje")]
        public DateTime RegistrationTime { get; set; }

        /// <summary>
        /// Projects a <see cref="TrashTrackerUser"/> to a <see cref="UserIndex"/> object.
        /// </summary>
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
