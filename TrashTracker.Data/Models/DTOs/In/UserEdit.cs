using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class UserEdit
    {
        public String Id { get; set; }

        [DisplayName("Felhasználónév")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String NewUserName { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail cím")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím!")]
        [Required(ErrorMessage = "E-mail cím megadása kötelező!")]
        public String Email { get; set; } = null!;

        [DisplayName("Szerepkör")]
        public Roles Role { get; set; }

        [DisplayName("Profilkép")]
        public Uri? ImageUrl { get; set; }

        [DisplayName("Profilkép")]
        [ValidImage]
        public IFormFile? Image { get; set; }

        public UserEdit() { }
        public UserEdit(TrashTrackerUser user, Roles role, String imageUrlBase)
        {
            Id = user.Id;
            NewUserName = user.UserName!;
            Email = user.Email!;
            Role = role;
            ImageUrl = user.Image != null
                ? new Uri($"{imageUrlBase}/{user.Image.Id}")
                : null;
        }
    }
}
