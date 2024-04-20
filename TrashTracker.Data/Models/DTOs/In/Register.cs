using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class Register : NavigationUrls
    {
        [DisplayName("Név")]
        public String UserName { get; set; } = null!;

        [DisplayName("Jelszó")]
        [DataType(DataType.Password)]
        public String Password { get; set; } = null!;

        [DisplayName("Jelszó megerősítése")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public String PasswordRepeat { get; set; } = null!;
    }
}
