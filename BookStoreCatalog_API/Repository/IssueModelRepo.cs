using BookStoreCatalog_API.Data;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Repository.IRepository;

namespace BookStoreCatalog_API.Repository
{
    public class IssueModelRepo : Repository<IssueModel>, IIssueRepo
    {

        private readonly ApplicationDbContext _dbContext;

        public IssueModelRepo(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IssueModel> UpdateBook(IssueModel entity)
        {
            entity.CreatedAt = DateTime.Now;
            _dbContext.Issues.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
