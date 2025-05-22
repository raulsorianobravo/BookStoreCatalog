using BookStoreCatalog_API.Models;

namespace BookStoreCatalog_API.Repository.IRepository
{
    public interface IIssueRepo : IRepository<IssueModel>
    {
        Task<IssueModel> UpdateBook(IssueModel entity);

    }
}
