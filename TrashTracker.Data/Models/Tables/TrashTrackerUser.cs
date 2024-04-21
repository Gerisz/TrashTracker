using Microsoft.AspNetCore.Identity;
using TrashTracker.Data.Models.DTOs.In;

namespace TrashTracker.Data.Models.Tables
{
    public class TrashTrackerUser : IdentityUser
    {
        public TrashTrackerUser() { }
        public TrashTrackerUser(Register register, Boolean? emailConfirmed = false)
        {
            UserName = register.UserName;
            Email = register.Email;
            EmailConfirmed = false;
        }
    }
}
