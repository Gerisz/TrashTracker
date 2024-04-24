using Microsoft.EntityFrameworkCore;

namespace TrashTracker.Web.Utils
{
    public class PaginatedList<T> : List<T>
    {
        public Int32 PageNumber { get; private set; }
        public Int32 PageSize { get; private set; }
        public Int32 TotalPages { get; private set; }

        public PaginatedList(List<T> items, Int32 count, Int32 pageNumber, Int32 pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (Int32)Math.Ceiling(count / (Double)pageSize);
            AddRange(items);
        }

        public Boolean HasPreviousPage => PageNumber > 1;

        public Boolean HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, Int32 pageIndex, Int32 pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
