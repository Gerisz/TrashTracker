using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Tables
{
    public class UserImage
    {
        [Key]
        public Int32 Id { get; set; }

        public virtual TrashTrackerUser? User { get; set; }

        public byte[]? Image { get; set; }
        public String ContentType { get; set; } = null!;

        public UserImage () { }
        public UserImage(IFormFile image)
        {
            using (var stream = new MemoryStream())
            {
                image.CopyTo(stream);
                Image = stream.ToArray();
            }
            ContentType = image.ContentType;
        }
    }
}
