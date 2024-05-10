using Microsoft.AspNetCore.Identity;
using TrashTracker.Data.Models.DTOs.In;

namespace TrashTracker.Data.Models.Tables
{
    public class TrashTrackerUser : IdentityUser
    {
        public Int32? ImageId { get; set; }
        public virtual UserImage? Image { get; set; }

        public DateTime RegistrationTime { get; set; }

        public TrashTrackerUser() { }

        public TrashTrackerUser(UserRegister register, Boolean? emailConfirmed = false)
        {
            UserName = register.UserName;
            Email = register.Email;
            EmailConfirmed = emailConfirmed ?? false;
            Image = register.Image != null
                ? new UserImage(register.Image)
                : null;
            RegistrationTime = DateTime.UtcNow;
        }

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
