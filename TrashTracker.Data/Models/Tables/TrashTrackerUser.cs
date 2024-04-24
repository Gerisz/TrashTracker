using Microsoft.AspNetCore.Identity;
using TrashTracker.Data.Models.DTOs.In;

namespace TrashTracker.Data.Models.Tables
{
    public class TrashTrackerUser : IdentityUser
    {
        public Int32? ImageId { get; set; }
        public virtual UserImage? Image { get; set; }

        public TrashTrackerUser() { }
        public TrashTrackerUser(Register register, Boolean? emailConfirmed = false)
        {
            UserName = register.UserName;
            Email = register.Email;
            EmailConfirmed = emailConfirmed ?? false;
            Image = register.Image != null
                ? new UserImage(register.Image) : null;
        }
    }
}
