using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    /// <summary>
    /// DTO meant to contain login information of a user.
    /// </summary>
    public class UserLogin 
    {
        /// <summary>
        /// Username of the user to be logged into.
        /// </summary>
        [DisplayName("Felhasználónév vagy E-mail cím")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String UserName { get; set; } = null!;

        /// <summary>
        /// Password associated to the user, defined by <see cref="UserName"/>.
        /// </summary>
        [DataType(DataType.Password)]
        [DisplayName("Jelszó")]
        [Required(ErrorMessage = "Jelszó megadása kötelező!")]
        public String Password { get; set; } = null!;
    }
}
