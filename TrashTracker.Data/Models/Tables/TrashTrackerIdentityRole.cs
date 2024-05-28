using Microsoft.AspNetCore.Identity;

namespace TrashTracker.Data.Models.Tables
{
    /// <summary>
    /// Definition of the table named AspNetRoles derived from <see cref="IdentityRole"/>.
    /// </summary>
    public class TrashTrackerIdentityRole : IdentityRole
    {
        public TrashTrackerIdentityRole() : base() { }

        public TrashTrackerIdentityRole(string roleName) : base(roleName) { }
    }
}
