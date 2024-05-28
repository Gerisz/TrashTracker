using Microsoft.EntityFrameworkCore;

namespace TrashTracker.Web.Utils
{
    /// <summary>
    /// A paginated list, which can be created from a <see cref="List{T}"/>,
    /// calculating given a page size,
    /// which objects should <see cref="this"/> contain on a certain number of page.
    /// </summary>
    /// <typeparam name="T">Type of object contained by <see cref="this"/>.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// The number of the page.
        /// </summary>
        public Int32 PageNumber { get; private set; }

        /// <summary>
        /// The size of the page, a. k. a. how many objects does it contain
        /// </summary>
        public Int32 PageSize { get; private set; }

        /// <summary>
        /// Total number of pages.
        /// </summary>
        public Int32 TotalPages { get; private set; }

        /// <summary>
        /// Creates an <see cref="PaginatedList{T}"/>, from the parameters.
        /// </summary>
        /// <param name="items">
        /// Which <see cref="List{T}"/> to create <see cref="this"/> from.
        /// </param>
        /// <param name="count">How many objects does the <paramref name="items"/> contains.</param>
        /// <param name="pageNumber">Which page should be contained in <see cref="this"/>.</param>
        /// <param name="pageSize">How many objects does <see cref="this"/> should contain.</param>
        public PaginatedList(List<T> items, Int32 count, Int32 pageNumber, Int32 pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (Int32)Math.Ceiling(count / (Double)pageSize);
            AddRange(items);
        }

        /// <summary>
        /// Logical value if this page has a previous one containing any elements.
        /// </summary>
        public Boolean HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Logival value if this page has a next one containing any elements.
        /// </summary>
        public Boolean HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Creates a <see cref="PaginatedList{T}"/> from an <see cref="IQueryable{T}"/>,
        /// given the page's number and size.
        /// </summary>
        /// <param name="source">
        /// Which <see cref="IQueryable{T}"/> to create <see cref="this"/> from.
        /// </param>
        /// <param name="pageIndex">The number of page.</param>
        /// <param name="pageSize">How many items should a page contain.</param>
        /// <returns>
        /// The <see cref="PaginatedList{T}"/> containing the given page's elements.
        /// </returns>
        public static async Task<PaginatedList<T>> CreateAsync
            (IQueryable<T> source, Int32 pageIndex, Int32 pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
