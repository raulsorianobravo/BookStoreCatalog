using BookStoreCatalog_API.Models.DTO;

namespace BookStoreCatalog_API.DataStore
{
    public class BookDataStore
    {
        public List<BookModelDTO> bookList = new List<BookModelDTO> 
        {
            new BookModelDTO{Id=991,Title="Test5"},
            new BookModelDTO{Id=992,Title="Test6"}            
        };

        public BookDataStore() { }
    }
}
