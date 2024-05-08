using CleanTiszaMap.Data.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class TrashFromUser
    {
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [DisplayName("Szélesség")]
        [Localizable(false)]
        [Range(-180.0, 180.0,
            ErrorMessage = "A koordináta nem lehet kevesebb, mint {0}° és több, mint {1}°.")]
        [Required(ErrorMessage = "Szélesség megadása kötelező!")]
        public Double Lat { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [DisplayName("Hosszúság")]
        [Localizable(false)]
        [Range(-180.0, 180.0,
            ErrorMessage = "A koordináta nem lehet kevesebb, mint -180 és több, mint 180.")]
        [Required(ErrorMessage = "Hosszúság megadása kötelező!")]
        public Double Long { get; set; }

        [DisplayName("Település")]
        public String? Locality { get; set; }

        [DisplayName("Településrész")]
        public String? SubLocality { get; set; }

        #region Accessibilities

        public Boolean ByCar { get; set; }
        public Boolean InCave { get; set; }
        public Boolean UnderWater { get; set; }
        public Boolean NotForGeneralCleanup { get; set; }

        #endregion

        [DisplayName("Méret")]
        public Size Size { get; set; }

        #region Types

        public Boolean Automotive { get; set; }
        public Boolean Construction { get; set; }
        public Boolean Dangerous { get; set; }
        public Boolean DeadAnimals { get; set; }
        public Boolean Domestic { get; set; }
        public Boolean Electronic { get; set; }
        public Boolean Glass { get; set; }
        public Boolean Liquid { get; set; }
        public Boolean Metal { get; set; }
        public Boolean Organic { get; set; }
        public Boolean Plastic { get; set; }

        #endregion

        [DisplayName("Megjegyzés")]
        [StringLength(2000, ErrorMessage = "{0} karakternél nem lehet hosszabb a megjegyzés!")]
        public String? Note { get; set; }

        [DisplayName("Képek hozzáadása")]
        [ValidImage]
        public IEnumerable<IFormFile?> Images { get; set; } = [];

        public TrashFromUser() { }

        protected TrashFromUser(Trash trash)
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
    }
}
