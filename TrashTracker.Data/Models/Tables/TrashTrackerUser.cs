using Microsoft.AspNetCore.Identity;
using TrashTracker.Data.Models.DTOs.In;

namespace TrashTracker.Data.Models.Tables
{
    /// <summary>
    /// Definition of the table named AspNetUsers derived from <see cref="IdentityUser"/>.
    /// </summary>
    public class TrashTrackerUser : IdentityUser
    {
        /// <summary>
        /// The id of the user's profile picture in the database.
        /// </summary>
        public Int32? ImageId { get; set; }

        /// <summary>
        /// The user's profile picture. 
        /// </summary>
        public virtual UserImage? Image { get; set; }

        /// <summary>
        /// The date and time of the user's registration (0000. 00. 00. if it's a default account).
        /// </summary>
        public DateTime RegistrationTime { get; set; }

        public TrashTrackerUser() { }

        /// <summary>
        /// Creates a <see cref="TrashTrackerUser"/> from a <see cref="UserRegister"/>.
        /// </summary>
        /// <param name="register">The user's data to create the user's object from.</param>
        /// <param name="emailConfirmed">Should the e-mail be confirmed by default or not.</param>
        public TrashTrackerUser(UserRegister register, Boolean emailConfirmed = false)
        {
            UserName = register.UserName;
            Email = register.Email;
            EmailConfirmed = emailConfirmed;
            Image = register.Image != null
                ? new UserImage(register.Image)
                : null;
            RegistrationTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates <see langword="this"/>' values with <paramref name="user"/>'s.
        /// </summary>
        /// <param name="userManager">
        /// The <see cref="UserManager{TrashTrackerUser}"/> containing the <paramref name="user"/>
        /// </param>
        /// <param name="user">
        /// An instance of <see cref="TrashTrackerUser"/> with the new values.
        /// </param>
        /// <returns>Returns <see langword="this"/> with it's updated values.</returns>
        public async Task<TrashTrackerUser> UpdateAsync(UserManager<TrashTrackerUser> userManager,
            UserEdit user)
        {
            if (UserName != user.NewUserName)
                await userManager.SetUserNameAsync(this, user.NewUserName);
            if (Email != user.Email)
                await userManager.SetEmailAsync(this, user.Email);
            if (user.Image != null && Image != user.Image)
                Image = new UserImage(user.Image);

            return this;
        }
    }
}
