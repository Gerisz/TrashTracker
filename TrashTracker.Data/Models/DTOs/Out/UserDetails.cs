using System.ComponentModel;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Utils;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class UserDetails
    {
        /// <summary>
        /// User's id in the database.
        /// </summary>
        public String UserId { get; set; } = null!;

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
        /// Logical value of the e-mail address' confirmation.
        /// </summary>
        public Boolean EmailConfirmed { get; set; }

        /// <summary>
        /// Name of the role the user is in
        /// (possible values defined by <see cref="Defaults.Roles"/>.
        /// </summary>
        public String Role { get; set; } = null!;

        /// <summary>
        /// Full download URL of the user's profile picture.
        /// </summary>
        public Uri? Image { get; set; }

        /// <summary>
        /// List of trashes created by the user.
        /// </summary>
        public PaginatedList<Trash>? Trashes { get; set; }

        /// <summary>
        /// Projects a <see cref="TrashTrackerUser"/> to a <see cref="UserDetails"/> object.
        /// </summary>
        /// <param name="role">The current role of <paramref name="user"/>.</param>
        /// <param name="imageUrlBase">
        /// The base of the image's URL (which endpoint the image can be downloaded from).
        /// </param>
        /// <param name="trashes">The trashes created by the user.</param>
        public static Expression<Func<TrashTrackerUser, UserDetails>> Projection
            (String role, String imageUrlBase, PaginatedList<Trash>? trashes)
            => user => new UserDetails()
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                Role = role,
                Image = user.Image != null
                    ? new Uri($"{imageUrlBase}/{user.Image.Id}")
                    : null,
                Trashes = trashes
            };

        /// <summary>
        /// Creates a <see cref="UserDetails"/> from a <see cref="TrashTrackerUser"/> object.
        /// </summary>
        /// <param name="user">The user to create the object from.</param>
        /// <param name="role">The current role of <paramref name="user"/>.</param>
        /// <param name="imageUrlBase">
        /// The base of the image's URL (which endpoint the image can be downloaded from).
        /// </param>
        /// <param name="trashes">The trashes created by the user.</param>
        /// <returns>The <see cref="UserDetails"/> object created from the parameters.</returns>
        public static UserDetails Create(TrashTrackerUser user, String role,
            String imageUrlBase, PaginatedList<Trash>? trashes)
            => Projection(role, imageUrlBase, trashes).Compile().Invoke(user);
    }
}
