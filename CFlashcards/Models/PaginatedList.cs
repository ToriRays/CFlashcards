using Microsoft.EntityFrameworkCore;

namespace CFlashcards.Models
{
    // Class used for pagination on the webpage. The functionality of the class is that the amount
    // of objects of type <T> inside this list will be equal to page size. The objects are chosen
    // from the source variable, by using the pageIndex to skip objects from previous intervals.
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; } // Number corresponding to the page number in the pagination.
        public int TotalPages { get; set; } // Number corresponding to the amount of pages given pageSize and source.Count.
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items); //
        }
        // Used to activate Previous button in BrowseDecks and BrowseFlashcards if true.
        public bool HasPreviousPage => (PageIndex > 1);
        // Used to activate Next button in BrowseDecks and BrowseFlashcards if true.
        public bool HasNextPage => (PageIndex < TotalPages);

        public static PaginatedList<T>? Create(List<T> source, int pageIndex, int pageSize)
        {
            try
            {
                var count = source.Count;
                // The line below selects the correct interval of objects <T> given the pageIndex and pageSize.
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}