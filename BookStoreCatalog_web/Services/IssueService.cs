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
            _issueURL = configuration.GetValue<string>("ServiceURL:API_URL");
        }

        public Task<T> CreateIssue<T>(IssueModelCreateDTO issueDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.POST,
                Data = issueDTO,
                URL = _issueURL + "/api/Issue/DbAPIResponse"
            });
        }

        public Task<T> DeleteIssue<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.DELETE,
                URL = _issueURL + "/api/Issue/DbAPIResponse/" + id
            });
        }

        public Task<T> GetAllIssues<T>()
        {
            return SendAsync<T>(new APIRequest() 
            { 
                APIType = ClassDefinitions.APIType.GET,
                URL = _issueURL + "/api/Issue/DbAPIResponse/"
            });
        }

        public Task<T> GetIssue<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.GET,
                URL = _issueURL + "/api/Issue/DbAPIResponse/" + id
            });
        }

        public Task<T> UpdateIssue<T>(IsssueModelUpdateDTO issueDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = ClassDefinitions.APIType.PUT,
                Data = issueDTO,
                URL = _issueURL + "/api/Issue/DbAPIResponse/" + issueDTO.IssueId
            });
        }
    }
}
