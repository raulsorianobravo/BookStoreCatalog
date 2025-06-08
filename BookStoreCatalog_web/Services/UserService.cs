using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalogUtils;

namespace BookStoreCatalog_web.Services
{
    public class UserService : BaseService, IUserService
    {

        public readonly IHttpClientFactory _httpClient;
        private string _userURL;

        public UserService(IHttpClientFactory httpClient, IConfiguration configuration) :base(httpClient)
        {
            _httpClient = httpClient;
            _userURL = configuration.GetValue<string>("ServiceURLs:API_URL");
        }

        public Task<T> Login<T>(LoginRequestDTO userDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                Data = userDTO,
                URL = _userURL + "/api/user/login"
            });
        }

        public Task<T> Register<T>(RegisterRequestDTO userDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                Data = userDTO,
                URL = _userURL + "/api/user/register"
            });
        }
    }
}
