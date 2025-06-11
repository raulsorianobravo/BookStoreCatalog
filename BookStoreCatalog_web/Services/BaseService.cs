using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Services.IServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

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
                message.Headers.Add("Accept", "application/json");

                if (apiRequest.Parameters != null)
                {
                    var builder = new UriBuilder(apiRequest.URL);
                    var query = HttpUtility.ParseQueryString(builder.Query);

                    query["PageNumber"] = apiRequest.Parameters.PageNumber.ToString();
                    query["PageSize"] = apiRequest.Parameters.PageSize.ToString();

                    builder.Query = query.ToString();
                    string url = builder.ToString();

                    //--- api/Book/GetAllPaged/PageNumber=1&PageSize=4

                    message.RequestUri = new Uri(url);
                }
                else
                { 
                    message.RequestUri = new Uri(apiRequest.URL); 
                }

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

                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",apiRequest.Token);
                }
                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                //var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

                try
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if (response !=null && (response.StatusCode == HttpStatusCode.BadRequest 
                                        || apiResponse.StatusCode == HttpStatusCode.NotFound))
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(response);
                        var obj = JsonConvert.DeserializeObject<T>(res);
                        return obj;
                    }
                }
                catch (Exception)
                {
                    var errorResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return errorResponse;
                }

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
                var apiResponse = JsonConvert.DeserializeObject<T>(res);
                return apiResponse;
            }


        }
    }
}
