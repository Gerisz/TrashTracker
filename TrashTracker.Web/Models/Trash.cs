using CleanTiszaMap.Data.Models;
using CleanTiszaMap.Data.Models.EnumModels;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Web.Models.EnumModels;

namespace TrashTracker.Web.Models
{
    [Index(nameof(TrashoutId), IsUnique = true)]
    public class Trash
    {
        public Int32 Id { get; set; }
        public Int32? TrashoutId { get; set; }
        public String? UserId { get; set; }
        public virtual TrashTrackerUser? User { get; set; }

        public Point? Location { get; set; } = null!;

        public Country? Country { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Boolean? UpdateNeeded { get; set; }

        [DataType(DataType.MultilineText)]
        public String? Note { get; set; } = null!;

        public Status Status { get; set; }
        public Size Size { get; set; }
        public TrashType Types { get; set; }
        public Accessibility Accessibilities { get; set; }

        public virtual List<TrashImage> Images { get; set; } = new();
    }
}
