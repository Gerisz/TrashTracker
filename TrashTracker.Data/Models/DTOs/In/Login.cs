using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class Login : NavigationUrls
    {
        [DisplayName("Név")]
        public String UserName { get; set; } = null!;

        [DisplayName("Jelszó")]
        [DataType(DataType.Password)]
        public String Password { get; set; } = null!;
    }
}
