using System.ComponentModel;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class TrashEdit : TrashFromUser
    {
        public Int32 Id { get; set; }
        public Int32? TrashoutId { get; set; }
        [DisplayName("Állapot")]
        public Status Status { get; set; }

        public TrashEdit() : base() { }
        public TrashEdit(Trash trash) : base(trash)
        {
            Id = trash.Id;
            TrashoutId = trash.TrashoutId;
            Status = trash.Status;
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
