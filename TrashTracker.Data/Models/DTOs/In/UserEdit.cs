using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class UserEdit : Register
    {
        [DisplayName("Szerepkör")]
        public Roles Role { get; set; }
        [DisplayName("Profilkép")]
        public Uri? ImageUrl { get; set; }

        public UserEdit() { }
        public UserEdit(TrashTrackerUser user, Roles role, String imageUrlBase) : base(user)
        {
            Role = role;
            ImageUrl = user.Image != null
                ? new Uri($"{imageUrlBase}/{user.Image.Id}")
                : null;
        }
    }
}
