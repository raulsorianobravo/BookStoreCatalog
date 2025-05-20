using BookStoreCatalog_API.Models;

namespace BookStoreCatalog_API.Repository.IRepository
{
    public interface IBookRepo : IRepository<BookModel>
    {
        Task<BookModel> UpdateBook(BookModel entity);
    }
}
