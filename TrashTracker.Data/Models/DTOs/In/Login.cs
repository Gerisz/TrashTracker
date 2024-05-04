using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class Login 
    {
        [DisplayName("Felhasználónév vagy E-mail cím")]
        [Required(ErrorMessage = "Felhasználónév megadása kötelező!")]
        public String UserName { get; set; } = null!;

        [DataType(DataType.Password)]
        [DisplayName("Jelszó")]
        [Required(ErrorMessage = "Jelszó megadása kötelező!")]
        public String Password { get; set; } = null!;
    }
}
