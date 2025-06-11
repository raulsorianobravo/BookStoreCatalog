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

        public Task<T> CreateBook<T>(BookModelCreateDTO bookDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                Data = bookDTO,
                URL = _bookURL+ "/api/v1/Book/DbAPIResponse",
                Token = token
            });
        }

        public Task<T> DeleteBook<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.DELETE,
                URL = _bookURL + "/api/v1/Book/DbAPIResponse/" + id,
                Token = token
            });
        }

        public Task<T> GetBook<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.GET,
                URL = _bookURL + "/api/v1/Book/DbAPIResponse/" + id,
                Token = token
            });
        }

        public Task<T> GetAllBooks<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.GET,
                URL = _bookURL + "/api/v1/Book/DbAPIResponse",
                Token = token
            });
        }

        public Task<T> UpdateBook<T>(BookModelUpdateDTO bookDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.PUT,
                Data = bookDTO,
                URL = _bookURL + "/api/v1/Book/DbAPIResponse/" + bookDTO.Id,
                Token = token
            });
        }

        public Task<T> GetAllBooksPaged<T>(string token, int pageNumber = 1, int pageSize = 4)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.GET,
                URL = _bookURL + "/api/v1/Book/DbAPIResponsePaged/",
                Token = token,
                Parameters = new Parameters() { PageNumber = pageNumber, PageSize = pageSize }
            });
        }
    }
}
