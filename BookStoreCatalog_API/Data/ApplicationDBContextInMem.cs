using BookStoreCatalog_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreCatalog_API.Data
{
    public class ApplicationDBContextInMem(DbContextOptions<ApplicationDBContextInMem> options) : DbContext(options)
    {
        public DbSet<BookModel> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<BookModel>().HasData(
                new BookModel()
                {
                    Id = 1,
                    idBook = "1",
                    Title = "Title991",
                    TitleUrl = "URL991",
                    Author = "Author991",
                    AuthorUrl = "URL991",
                    Description = "Description991",
                    DescriptionUrl = "URL991",
                    CreatedAt = DateTime.Now,
                }
            );


        }
    }
}
