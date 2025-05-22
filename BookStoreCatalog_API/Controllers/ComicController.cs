using BookStoreCatalog_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreCatalog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComicController : ControllerBase
    {
        //----------------------------------------------
        /// <summary>
        /// (TEST) Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("Comic/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookModel>> GetAllTest()
        {
            return Ok(
                new List<BookModel>
                {
                    new BookModel { Id = 99, Title = "Test1" },
                    new BookModel { Id = 999, Title = "Test2" }
                });
        }
    }
}
