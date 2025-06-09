using BookStoreCatalog_web.Models.DTO;

namespace BookStoreCatalog_web.Services.IServices
{
    public interface IIssueService
    {
        Task<T> GetAllIssues<T>(string token);
        Task<T> GetIssue<T>(int id, string token);

        Task<T> CreateIssue<T>(IssueModelCreateDTO issueDTO, string token);
        Task<T> UpdateIssue<T>(IsssueModelUpdateDTO issueDTO, string token);
        Task<T> DeleteIssue<T>(int id, string token);
    }
}
