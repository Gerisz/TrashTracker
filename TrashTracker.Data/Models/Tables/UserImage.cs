using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Tables
{
    /// <summary>
    /// The definition of the table named UserImages, containing a user's profile picture.
    /// </summary>
    public class UserImage
    {
        /// <summary>
        /// The id of the profile picture in the database.
        /// </summary>
        [Key]
        public Int32 Id { get; set; }

        /// <summary>
        /// The user whose profile picture is this.
        /// </summary>
        public virtual TrashTrackerUser? User { get; set; }

        /// <summary>
        /// Bytes containing the user's profile picture.
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// The file format of the profile picture.
        /// </summary>
        public String ContentType { get; set; } = null!;

        public UserImage () { }

        /// <summary>
        /// Created a <see cref="UserImage"/> from an <see cref="IFormFile"/>.
        /// </summary>
        /// <param name="image">The <see cref="IFormFile"/> to create the object from.</param>
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
