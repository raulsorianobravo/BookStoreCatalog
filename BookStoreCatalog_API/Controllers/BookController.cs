using BookStoreCatalog_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreCatalog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        //----------------------------------------------

        //----------------------------------------------

        /// <summary>
        /// Constructor - Injection
        /// </summary>
        public BookController()
        {
            
        }

        //----------------------------------------------
        /// <summary>
        /// (TEST) Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet]
        public IEnumerable<BookModel> GetAllTest()
        {
            return new List<BookModel>
            {
                new BookModel { Id = 99, Title = "Test1" },
                new BookModel { Id = 999, Title = "Test2" }
            };
        }
    }
}
