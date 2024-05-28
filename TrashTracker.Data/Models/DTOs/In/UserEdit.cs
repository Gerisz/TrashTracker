using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    /// <summary>
    /// DTO meant to contain every editable property of a <see cref="TrashTrackerUser"> object.
    /// </summary>
    public class UserEdit
    {
        /// <summary>
        /// The id of the user being edited.
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// The new username to be given to the user.
        /// </summary>
        [DisplayName("Felhasználónév")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String NewUserName { get; set; } = null!;

        /// <summary>
        /// The new e-mail address to be given to the user.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail cím")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím!")]
        [Required(ErrorMessage = "E-mail cím megadása kötelező!")]
        public String Email { get; set; } = null!;

        /// <summary>
        /// The new role to be given to the user (possible values defined by <see cref="Roles"/>).
        /// </summary>
        [DisplayName("Szerepkör")]
        public Roles Role { get; set; }

        /// <summary>
        /// The current profile picture's full download URL.
        /// </summary>
        [DisplayName("Profilkép")]
        public Uri? ImageUrl { get; set; }

        /// <summary>
        /// The new profile picture to be given to the user (if any),
        /// otherwise it should stay the same
        /// </summary>
        [DisplayName("Profilkép")]
        [ValidImage]
        public IFormFile? Image { get; set; }

        public UserEdit() { }

        /// <summary>
        /// Creates a <see cref="UserEdit"/> object
        /// from the given <see cref="TrashTrackerUser"/> object.
        /// </summary>
        /// <param name="user">
        /// The <see cref="TrashTrackerUser"/> object to copy values from.
        /// </param>
        /// <param name="role">The current role of <paramref name="user"/>.</param>
        /// <param name="imageUrlBase">
        /// The base of the image's URL (which endpoint the image can be downloaded from).
        /// </param>
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
