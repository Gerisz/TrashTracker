using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.Tables
{
    public class TrashImage
    {
        [Key]
        public Int32 Id { get; set; }

        public Uri? Url { get; set; }

        public String? UserId { get; set; }
        public virtual TrashTrackerUser? User { get; set; }
        public byte[]? Image { get; set; }

        public Int32 TrashId { get; set; }
        public virtual Trash? Trash { get; set; }

        public TrashImage(Uri url)
        {
            Url = url;

            UserId = null;
            Image = null;
        }

        public TrashImage(String userId, IFormFile image)
        {
            Url = null;

            UserId = userId;
            using (var stream = new MemoryStream())
            {
                image.CopyTo(stream);
                Image = stream.ToArray();
            }
        }
    }
}
