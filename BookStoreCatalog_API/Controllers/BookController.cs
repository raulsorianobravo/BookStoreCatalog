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
        [HttpGet("TEST/{id}:int")]
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
        [HttpGet("TEST/{storage}")]
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
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookModelDTO> CreateBook( [FromBody] BookModelDTO newBook)
        {
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
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
                if (BookDataStore.bookList.Any(book => book.Title.ToLower() == newBook.Title.ToLower()))
                {
                    ModelState.AddModelError("SameBook", "This Book Exists");
                    return BadRequest(ModelState);
                }
                BookDataStore.bookList.Add(newBook);
            }
            return CreatedAtRoute("GetBook", new { id = newBook.Id }, newBook);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("{id}:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBook(int id)
        {
            if (id == 0) 
            { 
                return BadRequest(); 
            }
            var book = BookDataStore.bookList.FirstOrDefault(book => book.Id == id);

            if ( book == null)
            {
                return NotFound();
            }
            else
            {
                BookDataStore.bookList.Remove(book);
            }
            return NoContent();


        }
        //----------------------------------------------
        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPut("{id}:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public IActionResult UpdateBook(int id, [FromBody] BookModelDTO modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                return BadRequest();
            }

            var book = BookDataStore.bookList.FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            book.Author = modBook.Author;
            book.Title = modBook.Title;
            book.Description = modBook.Description;
            book.AuthorUrl = modBook.AuthorUrl;
            book.DescriptionUrl = modBook.DescriptionUrl;
            book.TitleUrl = modBook.TitleUrl;

            return NoContent();
        }

        //----------------------------------------------

    }
}
