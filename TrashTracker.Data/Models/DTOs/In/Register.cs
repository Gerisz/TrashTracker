using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class Register
    {
        [DisplayName("Felhasználónév")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String UserName { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail cím")]
        [EmailAddress]
        [Required(ErrorMessage = "E-mail cím megadása kötelező!")]
        public String Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [DisplayName("Jelszó")]
        [Required(ErrorMessage = "Jelszó megadása kötelező!")]
        [StringLength(32,
            ErrorMessage = "A jelszó nem lehet rövidebb {2} és hosszabb {1} karakternél",
            MinimumLength = 6)]
        public String Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "Nem egyezik meg a megadott jelszóval!")]
        [DataType(DataType.Password)]
        [DisplayName("Jelszó megerősítése")]
        [Required(ErrorMessage = "Jelszó megerősítése kötelező!")]
        [StringLength(32,
            ErrorMessage = "A jelszó nem lehet rövidebb {2} és hosszabb {1} karakternél",
            MinimumLength = 6)]
        public String PasswordRepeat { get; set; } = null!;

        [DisplayName("Profilkép")]
        [ValidImage]
        public IFormFile? Image { get; set; }
    }
}
