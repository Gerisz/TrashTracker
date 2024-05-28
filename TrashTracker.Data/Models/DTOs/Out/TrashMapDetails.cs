using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class TrashMapDetails
    {
        public Int32 Id { get; set; }
        public String? Location { get; set; }
        public String? Note { get; set; }
        public IEnumerable<Uri> Images { get; set; } = [];

        public static Expression<Func<Trash, TrashMapDetails>> Projection(String imageUrlBase)
            => trash => new TrashMapDetails()
            {
                Id = trash.Id,
                Location = trash.SubLocality.IsNullOrEmpty()
                    ? trash.Locality
                    : trash.Locality.IsNullOrEmpty()
                        ? trash.SubLocality
                        : trash.Locality + ", " + trash.SubLocality,
                Note = trash.Note,
                Images = trash.Images
                    .ToList()
                    .Select(i => i.Url ?? new Uri($"{imageUrlBase}/{i.Id}"))
            };

        public static TrashMapDetails Create(Trash trash, String imageUrlBase)
            => Projection(imageUrlBase).Compile().Invoke(trash);
    }
}
