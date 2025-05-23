using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalogUtils;

namespace BookStoreCatalog_web.Services
{
    public class BookService : BaseService, IBookService
    {

        private readonly IHttpClientFactory _httpClient;
        private string _bookURL;

        public BookService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            _bookURL = configuration.GetValue<string>("ServiceURLs:API_URL");
        }

        public Task<T> CreateBook<T>(BookModelCreateDTO bookDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                Data = bookDTO,
                URL = _bookURL+"/api/Book"
            });
        }

        public Task<T> DeleteBook<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.DELETE,
                URL = _bookURL + "/api/Book" + id
            });
        }

        public Task<T> GetBook<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.GET,
                URL = _bookURL + "/api/Book"+id
            });
        }

        public Task<T> GetAllBooks<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                URL = _bookURL + "/api/Book"
            });
        }

        public Task<T> UpdateBook<T>(BookModelUpdateDTO bookDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.PUT,
                Data = bookDTO,
                URL = _bookURL + "/api/Book"+bookDTO.Id
            });
        }
    }
}
