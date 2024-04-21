using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class Login : NavigationUrls
    {
        [DisplayName("Felhasználónév vagy E-mail cím")]
        public String UserName { get; set; } = null!;

        [DataType(DataType.Password)]
        [DisplayName("Jelszó")]
        public String Password { get; set; } = null!;
    }
}
