using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class UserEdit
    {
        public String UserName { get; set; } = null!;
        public Roles Role { get; set; }

        public UserEdit() { }
        public UserEdit(TrashTrackerUser user, Roles role)
        {
            UserName = user.UserName;
            Role = role;
        }
    }
}
