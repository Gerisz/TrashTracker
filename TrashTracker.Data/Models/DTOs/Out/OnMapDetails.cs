using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class OnMapDetails
    {
        public Int32 Id { get; set; }
        public String? Location { get; set; }
        public String? Note { get; set; }
        public IEnumerable<Uri> Images { get; set; } = [];

        public static Expression<Func<Trash, OnMapDetails>> Projection(String imageUrlBase)
            => trash => new OnMapDetails()
            {
                Id = trash.Id,
                Location = trash.SubLocality.IsNullOrEmpty()
                    ? trash.Locality
                    : trash.Locality.IsNullOrEmpty()
                        ? trash.SubLocality
                        : trash.Locality + ", " + trash.SubLocality,
                Note = trash.Note,
                Images = trash.Images.Select(i => i.Url ?? new Uri($"{imageUrlBase}/{i.Id}"))
            };

        public static OnMapDetails Create(Trash place, String imageUrlBase)
            => Projection(imageUrlBase).Compile().Invoke(place);
    }
}
