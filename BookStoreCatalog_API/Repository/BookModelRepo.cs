using BookStoreCatalog_API.Data;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Repository.IRepository;

namespace BookStoreCatalog_API.Repository
{
    public class BookModelRepo : Repository<BookModel>, IBookRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public BookModelRepo(ApplicationDbContext dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BookModel> Update(BookModel entity)
        {
            entity.CreatedAt = DateTime.Now;
            _dbContext.Books.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
