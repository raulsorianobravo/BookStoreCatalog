using BookStoreCatalog_web.Models.DTO;
using System.Diagnostics.Eventing.Reader;

namespace BookStoreCatalog_web.ViewModel
{
    public class BookPagedViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string Prev { get; set; } = "disable";

        public string Next { get; set; } = "";

        public IEnumerable<BookModelDTO> BookList { get; set; }


    }
}
