using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class UserRegister
    {
        [DisplayName("Felhasználónév")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String UserName { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail cím")]
        [Required(ErrorMessage = "E-mail cím megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím!")]
        public String Email { get; set; } = null!;

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

        [Compare("Password", ErrorMessage = "Nem egyezik meg a megadott jelszóval!")]
        [DataType(DataType.Password)]
        [DisplayName("Jelszó megerősítése")]
        [Required(ErrorMessage = "Jelszó megerősítése kötelező!")]
        public String PasswordRepeat { get; set; } = null!;

        [DisplayName("Profilkép")]
        [ValidImage]
        public IFormFile? Image { get; set; }

        public UserRegister() { }
        public UserRegister(TrashTrackerUser user)
        {
            UserName = user.UserName!;
            Email = user.Email!;
        }
    }
}
