using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class Register 
    {
        [DisplayName("Felhasználónév")]
        public String UserName { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail cím")]
        [EmailAddress]
        public String Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [DisplayName("Jelszó")]
        public String Password { get; set; } = null!;

        [Compare("Password")]
        [DataType(DataType.Password)]
        [DisplayName("Jelszó megerősítése")]
        public String PasswordRepeat { get; set; } = null!;

        [DisplayName("Profilkép")]
        [ValidImage]
        public IFormFile? Image { get; set; }
    }
}
