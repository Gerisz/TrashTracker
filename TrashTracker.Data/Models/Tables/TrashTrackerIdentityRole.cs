using Microsoft.AspNetCore.Identity;

namespace TrashTracker.Data.Models.Tables
{
    public class TrashTrackerIdentityRole : IdentityRole
    {
        public TrashTrackerIdentityRole() : base() { }

        public TrashTrackerIdentityRole(string roleName) : base(roleName) { }
    }
}
