using Microsoft.AspNetCore.Identity;

namespace TrashTracker.Web.Models
{
    public class TrashTrackerIdentityRole : IdentityRole
    {
        public TrashTrackerIdentityRole() : base() { }

        public TrashTrackerIdentityRole(string roleName) : base(roleName) { }
    }
}
