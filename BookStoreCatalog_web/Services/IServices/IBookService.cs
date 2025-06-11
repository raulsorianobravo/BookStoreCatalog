using BookStoreCatalog_web.Models.DTO;

namespace BookStoreCatalog_web.Services.IServices
{
    public interface IBookService
    {
        Task<T> GetAllBooks<T>(string token);
        Task<T> GetBook<T>(int id, string token);

        Task<T> CreateBook<T>(BookModelCreateDTO bookDTO, string token);
        Task<T> UpdateBook<T>(BookModelUpdateDTO bookDTO, string token);
        Task<T> DeleteBook<T>(int id, string token);


        Task<T> GetAllBooksPaged<T>(string token, int pageNumber = 1, int pageSize = 4);

    }
}
