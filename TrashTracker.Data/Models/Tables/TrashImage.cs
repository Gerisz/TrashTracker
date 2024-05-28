using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Tables
{
    /// <summary>
    /// Definition of the table named TrashImages, 
    /// containing images linked to trash points.
    /// </summary>
    public class TrashImage
    {
        /// <summary>
        /// The id of the image in the database.
        /// </summary>
        [Key]
        public Int32 Id { get; set; }

        /// <summary>
        /// The full download URL of the image (if downloaded from TrashOut).
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// The id of the user uploading the image (if uploaded by user).
        /// </summary>
        public String? UserId { get; set; }

        /// <summary>
        /// The user uploading the image (if uploaded by user).
        /// </summary>
        public virtual TrashTrackerUser? User { get; set; }

        /// <summary>
        /// Bytes containing the image (if uploaded by the user).
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// The file format of the image (if uploaded by the user).
        /// </summary>
        public String? ContentType { get; set; }

        /// <summary>
        /// Id of the trash linked to the image.
        /// </summary>
        public Int32 TrashId { get; set; }

        /// <summary>
        /// The trash linked to the image.
        /// </summary>
        public virtual Trash? Trash { get; set; }

        /// <summary>
        /// Created a <see cref="TrashImage"/> from a <see cref="Uri"/>.
        /// </summary>
        /// <param name="url">The <see cref="Uri"/> to create the object with.</param>
        public TrashImage(Uri url)
        {
            Url = url;

            UserId = null;
            Image = null;
        }

        /// <summary>
        /// Created a <see cref="TrashImage"/> from an <see cref="IFormFile"/>.
        /// </summary>
        /// <param name="userId">User's id who uploads the image.</param>
        /// <param name="image">The <see cref="IFormFile"/> to create the object from.</param>
        public TrashImage(String userId, IFormFile image)
        {
            Url = null;

            UserId = userId;
            using (var stream = new MemoryStream())
            {
                image.CopyTo(stream);
                Image = stream.ToArray();
            }
            ContentType = image.ContentType;
        }
    }
}
