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

        [HttpGet]
        public IEnumerable<BookModel> GetAllTest()
        {
            return new List<BookModel>
            {
                new BookModel { Id = -1, Title = "Test1" },
                new BookModel { Id = -2, Title = "Test2" }
            };
        }
    }
}
