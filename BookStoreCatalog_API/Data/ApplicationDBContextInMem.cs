using BookStoreCatalog_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreCatalog_API.Data
{
    public class ApplicationDBContextInMem(DbContextOptions<ApplicationDBContextInMem> options) : DbContext(options)
    {
        public DbSet<BookModel> Books { get; set; }
    }
}
