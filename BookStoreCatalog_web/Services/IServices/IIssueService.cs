using BookStoreCatalog_web.Models.DTO;

namespace BookStoreCatalog_web.Services.IServices
{
    public interface IIssueService
    {
        Task<T> GetAllIssues<T>();
        Task<T> GetIssue<T>(int id);

        Task<T> CreateIssue<T>(IssueModelCreateDTO issueDTO);
        Task<T> UpdateIssue<T>(IsssueModelUpdateDTO issueDTO);
        Task<T> DeleteIssue<T>(int id);
    }
}
