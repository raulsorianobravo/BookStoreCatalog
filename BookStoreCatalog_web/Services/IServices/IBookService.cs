using BookStoreCatalog_web.Models.DTO;

namespace BookStoreCatalog_web.Services.IServices
{
    public interface IBookService
    {
        Task<T> GetAllBooks<T>();
        Task<T> GetBook<T>(int id);

        Task<T> CreateBook<T>(BookModelCreateDTO bookDTO);
        Task<T> UpdateBook<T>(BookModelUpdateDTO bookDTO);
        Task<T> DeleteBook<T>(int id);

    }
}
