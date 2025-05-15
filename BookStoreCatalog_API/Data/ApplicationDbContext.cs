using BookStoreCatalog_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreCatalog_API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<BookModel> Books { get; set; }

        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        //{            
        //}
    }
}
