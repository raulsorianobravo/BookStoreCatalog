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
                    CreatedAt = new DateTime(2000,1,1)
                },
                new BookModel()
                {
                    Id =2,
                    idBook = "2",
                    Title = "Title992",
                    TitleUrl = "URL992",
                    Author = "Author992",
                    AuthorUrl = "URL992",
                    Description = "Description992",
                    DescriptionUrl = "URL992",
                    CreatedAt = new DateTime(2000, 1, 1)
                }
            );
        }
    }
}
