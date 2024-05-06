using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CleanTiszaMap.Data.Attributes
{
    /// <summary>
    /// Validates an image. The image is considered valid, even if it's null.
    /// </summary>
    public class ValidImageAttribute : ValidationAttribute
    {
        public Int32 MaxSizeBytes { get; }

        public const Int32 DefaultMaxImageSizeBytes = 1 * 1024 * 1024; // 1 megabyte

        public readonly String[] AllowedExtensions = [ ".jpg", ".jpeg", ".png" ];

        /// <summary>
        /// A constructor for ValidImageAttribute that sets the maximum size of the image in bytes
        /// </summary>
        /// <param name="maxSizeBytes">The maximum size of the image in bytes.</param>
        public ValidImageAttribute(Int32 maxSizeBytes = DefaultMaxImageSizeBytes)
        {
            MaxSizeBytes = maxSizeBytes;
        }

        public override Boolean IsValid(Object? value)
        {
            if (value == null)
                return true;

            if (value is IFormFile file)
                return IsValidImage(file);
            else if (value is IEnumerable<IFormFile> files)
                return files.All(IsValidImage);

            ErrorMessage = "Az objektum nem fájl!";
            return false;
        }

        private Boolean IsValidImage(IFormFile file)
        {
            if (!file.ContentType.StartsWith("image/"))
            {
                ErrorMessage = "A fájl nem egy kép!";
                return false;
            }

            var extension = Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension))
            {
                var supportedFormats = String.Join(", ", AllowedExtensions);
                ErrorMessage = $"Ez a kép formátuma nem támogatott!" +
                    $"Támogatott formátumok: {supportedFormats}.";
                return false;
            }

            if (file.Length > MaxSizeBytes)
            {
                ErrorMessage = $"Ez a kép túl nagy!" +
                    $"Maximális méret: {MaxSizeBytes / 1024}KB.";
            }

            return true;
        }
    }
}
