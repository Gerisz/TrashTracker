using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    /// <summary>
    /// DTO meant to contain every relevant property
    /// when creating a <see cref="Trash"/> object by a user.
    /// </summary>
    public class TrashFromUser
    {
        /// <summary>
        /// Latitude of the location (must be between -180.0 and 180.0).
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [DisplayName("Szélesség")]
        [Localizable(false)]
        [Range(-180.0, 180.0,
            ErrorMessage = "A koordináta nem lehet kevesebb, mint {1}° és több, mint {2}°.")]
        [Required(ErrorMessage = "Szélesség megadása kötelező!")]
        public Double Lat { get; set; }

        /// <summary>
        /// Longitude of the location (must be between -180.0 and 180.0).
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [DisplayName("Hosszúság")]
        [Localizable(false)]
        [Range(-180.0, 180.0,
            ErrorMessage = "A koordináta nem lehet kevesebb, mint {1}° és több, mint {2}°.")]
        [Required(ErrorMessage = "Hosszúság megadása kötelező!")]
        public Double Long { get; set; }

        /// <summary>
        /// Locality of the location, typically in which settlement's borders is the point is in.
        /// </summary>
        [DisplayName("Település")]
        public String? Locality { get; set; }

        /// <summary>
        /// Sublocality of the location, typically in which district is the point is in.
        /// </summary>
        [DisplayName("Településrész")]
        public String? SubLocality { get; set; }

        #region Accessibilities

        /// <summary>
        /// Logical value of the point's accessibility by car.
        /// </summary>
        public Boolean ByCar { get; set; }

        /// <summary>
        /// Logical value of the point being in a cave.
        /// </summary>
        public Boolean InCave { get; set; }

        /// <summary>
        /// Logical value of the point being underwater.
        /// </summary>
        public Boolean UnderWater { get; set; }

        /// <summary>
        /// Logical value of the point not being able to be cleaned up by general means.
        /// </summary>
        public Boolean NotForGeneralCleanup { get; set; }

        #endregion

        /// <summary>
        /// The size of the trash
        /// defined by a value of <see cref="Enums.Size"/> <see langword="enum"/>.
        /// </summary>
        [DisplayName("Méret")]
        public Size Size { get; set; }

        #region Types

        /// <summary>
        /// Logical value of the trash containing automotive waste.
        /// </summary>
        public Boolean Automotive { get; set; }

        /// <summary>
        /// Logical value of the trash containing construction waste.
        /// </summary>
        public Boolean Construction { get; set; }

        /// <summary>
        /// Logical value of the trash containing dangerous waste.
        /// </summary>
        public Boolean Dangerous { get; set; }

        /// <summary>
        /// Logical value of the trash containing dead animals.
        /// </summary>
        public Boolean DeadAnimals { get; set; }

        /// <summary>
        /// Logical value of the trash containing domestic waste.
        /// </summary>
        public Boolean Domestic { get; set; }

        /// <summary>
        /// Logical value of the trash containing electronic waste.
        /// </summary>
        public Boolean Electronic { get; set; }

        /// <summary>
        /// Logical value of the trash containing glass.
        /// </summary>
        public Boolean Glass { get; set; }

        /// <summary>
        /// Logical value of the trash containing liquid waste.
        /// </summary>
        public Boolean Liquid { get; set; }

        /// <summary>
        /// Logical value of the trash containing metal.
        /// </summary>
        public Boolean Metal { get; set; }

        /// <summary>
        /// Logical value of the trash containing organic materials.
        /// </summary>
        public Boolean Organic { get; set; }

        /// <summary>
        /// Logical value of the trash containing plastic.
        /// </summary>
        public Boolean Plastic { get; set; }

        #endregion

        /// <summary>
        /// Additional information given about the trash
        /// (can't be more than 2000 characters).
        /// </summary>
        [DisplayName("Megjegyzés")]
        [StringLength(2000, ErrorMessage = "{0} karakternél nem lehet hosszabb a megjegyzés!")]
        public String? Note { get; set; }

        /// <summary>
        /// Images to be linked to the point, made about the trash
        /// (valid images are defined by <see cref="ValidImageAttribute"/>).
        /// </summary>
        [DisplayName("Képek hozzáadása")]
        [ValidImage]
        public IEnumerable<IFormFile?> Images { get; set; } = [];

        public TrashFromUser() { }

        /// <summary>
        /// Creates a <see cref="TrashFromUser"/> object from the given <see cref="Trash"/> object.
        /// </summary>
        /// <param name="trash">The <see cref="Trash"/> object to copy values from.</param>
        public TrashFromUser(Trash trash)
        {
            Lat = trash.Location.X;
            Long = trash.Location.Y;
            Locality = trash.Locality;
            SubLocality = trash.Locality;
            ByCar = (trash.Accessibilities & Accessibility.ByCar) != 0;
            InCave = (trash.Accessibilities & Accessibility.InCave) != 0;
            UnderWater = (trash.Accessibilities & Accessibility.UnderWater) != 0;
            NotForGeneralCleanup = (trash.Accessibilities & Accessibility.NotForGeneralCleanup) != 0;
            Size = trash.Size;
            Automotive = (trash.Types & TrashType.Automotive) != 0;
            Construction = (trash.Types & TrashType.Construction) != 0;
            Dangerous = (trash.Types & TrashType.Dangerous) != 0;
            DeadAnimals = (trash.Types & TrashType.DeadAnimals) != 0;
            Domestic = (trash.Types & TrashType.Domestic) != 0;
            Electronic = (trash.Types & TrashType.Electronic) != 0;
            Glass = (trash.Types & TrashType.Glass) != 0;
            Liquid = (trash.Types & TrashType.Liquid) != 0;
            Metal = (trash.Types & TrashType.Metal) != 0;
            Organic = (trash.Types & TrashType.Organic) != 0;
            Plastic = (trash.Types & TrashType.Plastic) != 0;
            Note = trash.Note;
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            return GetType().GetProperties().ToList()
                .Where(p => p.GetValue(this) != null && obj.GetType().GetProperty(p.Name)!.GetValue(obj) != null)
                .All(p => p.GetValue(this)!.Equals(obj.GetType().GetProperty(p.Name)!.GetValue(obj)));
        }
    }
}
