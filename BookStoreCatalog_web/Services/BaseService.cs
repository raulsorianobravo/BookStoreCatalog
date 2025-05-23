using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace BookStoreCatalog_web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get ; set ; }
        public IHttpClientFactory _httpClient {  get; set ; }   

        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new APIResponse();
            _httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {

            try
            {
                var client = _httpClient.CreateClient("BookStoreClient");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Content-Type", "application/json");
                message.RequestUri = new Uri(apiRequest.URL);

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),Encoding.UTF8, "application/json");
                }

                switch (apiRequest.APIType)
                {
                    case BookStoreCatalogUtils.ClassDefinitions.APIType.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case BookStoreCatalogUtils.ClassDefinitions.APIType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case BookStoreCatalogUtils.ClassDefinitions.APIType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case BookStoreCatalogUtils.ClassDefinitions.APIType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiResponse = null;
                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

                return APIResponse;
            }
            catch (Exception ex)
            {

                var dto = new APIResponse();
                dto.ErrorMessages = new List<string>();
                dto.ErrorMessages.Add(Convert.ToString(ex.Message));
                dto.IsSuccess = false;
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }


        }
    }
}
