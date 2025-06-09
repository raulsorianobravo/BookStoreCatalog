using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalogUtils;

namespace BookStoreCatalog_web.Services
{
    public class IssueService : BaseService, IIssueService
    {
        private readonly IHttpClientFactory _httpClient;
        private string _issueURL;

        public IssueService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            _issueURL = configuration.GetValue<string>("ServiceURLs:API_URL");
        }

        public Task<T> CreateIssue<T>(IssueModelCreateDTO issueDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                Data = issueDTO,
                URL = _issueURL + "/api/Issue/DbAPIResponse",
                Token = token
            });
        }

        public Task<T> DeleteIssue<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.DELETE,
                URL = _issueURL + "/api/Issue/DbAPIResponse/" + id,
                Token = token
            });
        }

        public Task<T> GetAllIssues<T>(string token)
        {
            return SendAsync<T>(new APIRequest() 
            { 
                APIType = ClassDefinitions.APIType.GET,
                URL = _issueURL + "/api/Issue/DbAPIResponse/",
                Token = token
            });
        }

        public Task<T> GetIssue<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.GET,
                URL = _issueURL + "/api/Issue/DbAPIResponse/" + id,
                Token = token
            });
        }

        public Task<T> UpdateIssue<T>(IsssueModelUpdateDTO issueDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.PUT,
                Data = issueDTO,
                URL = _issueURL + "/api/Issue/DbAPIResponse/" + issueDTO.IssueId,
                Token = token
            });
        }
    }
}
