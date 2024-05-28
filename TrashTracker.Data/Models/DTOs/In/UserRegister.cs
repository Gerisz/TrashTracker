using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    /// <summary>
    /// DTO meant to contain every relevant property
    /// when creating a <see cref="TrashTrackerUser"/> object by a user
    /// (a. k. a. when registering an account).
    /// </summary>
    public class UserRegister
    {
        /// <summary>
        /// Username of the user to be created with.
        /// </summary>
        [DisplayName("Felhasználónév")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String UserName { get; set; } = null!;

        /// <summary>
        /// E-mail address of the user to be created with (and be contacted at by other users).
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail cím")]
        [Required(ErrorMessage = "E-mail cím megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím!")]
        public String Email { get; set; } = null!;

        /// <summary>
        /// Password of the user to be associated with.
        /// </summary>
        [DataType(DataType.Password)]
        [DisplayName("Jelszó")]
        [Required(ErrorMessage = "Jelszó megadása kötelező!")]
        [StringLength(32,
            ErrorMessage = "A jelszó nem lehet rövidebb {2} és hosszabb {1} karakternél!",
            MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Jelszónak tartalmaznia kell legalább egy kis- és nagybetűt, " +
            "illetve számjegyet!")]
        public String Password { get; set; } = null!;

        /// <summary>
        /// Confirmation of the user's password 
        /// (must have the same value as <see cref="Password"/>).
        /// </summary>
        [Compare("Password", ErrorMessage = "Nem egyezik meg a megadott jelszóval!")]
        [DataType(DataType.Password)]
        [DisplayName("Jelszó megerősítése")]
        [Required(ErrorMessage = "Jelszó megerősítése kötelező!")]
        public String PasswordRepeat { get; set; } = null!;

        /// <summary>
        /// Profile picture of the user to be created with
        /// (valid images are defined by <see cref="ValidImageAttribute"/>).
        /// </summary>
        [DisplayName("Profilkép")]
        [ValidImage]
        public IFormFile? Image { get; set; }

        public UserRegister() { }
    }
}
