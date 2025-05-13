using BookStoreCatalog_API.DataStore;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;
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
        [HttpGet("TEST/")]
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

        //----------------------------------------------
        /// <summary>
        /// (TEST) Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DTO/{id}:int")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<BookModelDTO> GetAllTestDTO(int id)
        {
            return new List<BookModelDTO>
            {
                new BookModelDTO { Id = 99, Title = "Test3" },
                new BookModelDTO { Id = 999, Title = "Test4" }
            };
        }

        //----------------------------------------------
        /// <summary>
        /// (TEST) Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("Storage/{storage}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookModelDTO>> GetAllTestStorage(string storage)
        {
            return Ok(BookDataStore.bookList);
        }

        //----------------------------------------------
        /// <summary>
        ///  Get all the Books
        /// </summary>
        /// <returns> The list of Books </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookModel>> GetAll()
        {
            return Ok(BookDataStore.bookList);
        }

        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("{id}:int", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookModelDTO> GetBook(int id)
        {
            if (id == 0) 
            { 
                return BadRequest();
            }

            var book = BookDataStore.bookList.FirstOrDefault(book => book.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        //----------------------------------------------
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookModelDTO> CreateBook( [FromBody] BookModelDTO newBook)
        {
            if (newBook == null)
            {
                return BadRequest(newBook);
            }
            if (newBook.Id > 0)
            { 
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }

            else
            {
                newBook.Id = BookDataStore.bookList.OrderByDescending(book => book.Id).FirstOrDefault().Id + 1;
                if (BookDataStore.bookList.Any(
                    book => book.Id == newBook.Id
                ))
                {
                    return BadRequest(newBook);
                }
                BookDataStore.bookList.Add(newBook);
            }
            return CreatedAtRoute("GetBook", new { id = newBook.Id }, newBook);
        }
    }
}
