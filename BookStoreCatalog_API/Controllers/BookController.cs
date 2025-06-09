using AutoMapper;
using BookStoreCatalog_API.Data;
using BookStoreCatalog_API.DataStore;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;
using BookStoreCatalog_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookStoreCatalog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        //----------------------------------------------
        //--- Logger
        private readonly ILogger<BookController> _logger;

        //--- DbContext
        private readonly ApplicationDBContextInMem _context;

        //--- DbContext SQL Server
        private readonly ApplicationDbContext _dbContext;

        private readonly IMapper _mapper;

        private readonly IBookRepo _bookRepo;

        protected APIResponse _response;

        //----------------------------------------------

        /// <summary>
        /// Constructor - Injection
        /// </summary>
        public BookController(ILogger<BookController> logger, ApplicationDBContextInMem context, ApplicationDbContext dbContext, IMapper mapper, IBookRepo bookRepo)
        {
            _logger = logger;
            _context = context;
            _context.Database.EnsureCreated();
            _dbContext = dbContext;
            _mapper = mapper;
            _bookRepo = bookRepo;

            _response = new APIResponse();
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
            _logger.LogInformation("TEST Get all the books");
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
        [HttpGet("TEST/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<BookModelDTO> GetAllTestDTO(int id)
        {
            _logger.LogInformation("TEST Get all books from DTOs");
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
            _logger.LogInformation($"{storage}");
            _logger.LogInformation("TEST Get all books from Storage");
            return Ok(BookDataStore.bookList);
        }

        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> The list of Books </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookModel>> GetAll()
        {
            _logger.LogInformation("Get all the Books");
            return Ok(BookDataStore.bookList);
        }

        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("{id:int}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookModelDTO> GetBook(int id)
        {
            if (id == 0) 
            { 
                _logger.LogInformation($"{nameof(GetBook)}");
                _logger.LogError("Error: not valid ID");
                return BadRequest();
            }

            var book = BookDataStore.bookList.FirstOrDefault(book => book.Id == id);

            if (book == null)
            {
                _logger.LogError("Error: There's no Book with this ID");
                return NotFound();
            }
            _logger.LogInformation("Sucessful:"+ $"{book.Title}");
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
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }
            
            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }
            if (newBook.Id > 0)
            {
                _logger.LogError("Error: The ID is not valid");
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }

            else
            {
                newBook.Id = BookDataStore.bookList.OrderByDescending(book => book.Id).FirstOrDefault().Id + 1;
                if (BookDataStore.bookList.Any(book => book.Title.ToLower() == newBook.Title.ToLower()))
                {
                    ModelState.AddModelError("SameBook", "This Book Exists");

                    _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
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
        [HttpDelete("{id:int}")]
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
        [HttpPut("{id:int}")]
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
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdatePatchBook(int id, JsonPatchDocument <BookModelDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                return BadRequest();
            }

            var book = BookDataStore.bookList.FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            patchBook.ApplyTo(book, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
        //----------------------------------------------
        //               IN MEMORY
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("InMem/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookModel>> GetAllInMem()
        {
            _logger.LogInformation("TEST Get all the books");
            var books = _context.Books.ToList();
            return books;
                
        }

        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("InMem/{id:int}", Name = "GetBookInMem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookModelDTO> GetBookInMem(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetBookInMem)}");
                _logger.LogError("Error: not valid ID");
                return BadRequest();
            }

            var book = new BookModel();

            try
            {
                book = _context.Books.FirstOrDefault(book => book.Id == id);
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.Title}");
                    return Ok(book);
                }
                else
                {
                    _logger.LogError("Error: There's no Book with this ID");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("InMem/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookModel> CreateBookInMem([FromBody] BookModel newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }
            if (newBook.Id > 0)
            {
                _logger.LogError("Error: The ID is not valid");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            else
            {
                try
                {
                    int a = 0;
                    if(_context.Books.Count() > 0)
                        newBook.Id = _context.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                        
                    else newBook.Id = 1;    

                    if (_context.Books.Any(book => book.Title.ToLower() == newBook.Title.ToLower()))
                    {
                        ModelState.AddModelError("SameBook", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
                
            }

            _context.Books.Add(newBook);
            _context.SaveChanges();

            return CreatedAtRoute("GetBook", new { id = newBook.Id }, newBook);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("InMem/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBookInMem(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var book = _context.Books.FirstOrDefault(book => book.Id == id);    

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return NoContent();


        }

        //----------------------------------------------
        //               DataBase
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DB/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookModel>> GetAllDb()
        {
            _logger.LogInformation("Get all the books");
            var books = _dbContext.Books.ToList();
            return books;

        }
        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("DB/{id:int}", Name = "GetBookDb")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookModelDTO> GetBookDb(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetBookInMem)}");
                _logger.LogError("Error: not valid ID");
                return BadRequest();
            }

            var book = new BookModel();

            try
            {
                book = _dbContext.Books.FirstOrDefault(book => book.Id == id);
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.Title}");
                    return Ok(book);
                }
                else
                {
                    _logger.LogError("Error: There's no Book with this ID");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("DB/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookModel> CreateBookDb([FromBody] BookModel newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }
            if (newBook.Id > 0)
            {
                _logger.LogError("Error: The ID is not valid");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            else
            {
                try
                {
                    int a = 0;
                    //if (_dbContext.Books.Count() > 0)
                    //    newBook.Id = _dbContext.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                    //else newBook.Id = 1;

                    if (_dbContext.Books.Any(book => book.Title.ToLower() == newBook.Title.ToLower()))
                    {
                        ModelState.AddModelError("SameBook", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            _dbContext.Books.Add(newBook);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetBookDb", new { id = newBook.Id }, newBook);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DB/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBookDb(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var book = _dbContext.Books.FirstOrDefault(book => book.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _dbContext.Books.Remove(book);
                _dbContext.SaveChanges();
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
        [HttpPut("DB/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdateBookDb(int id, [FromBody] BookModel modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                return BadRequest();
            }

            var book = _dbContext.Books.FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            book.idBook = modBook.idBook;
            book.Author = modBook.Author;
            book.Title = modBook.Title;
            book.Description = modBook.Description;
            book.AuthorUrl = modBook.AuthorUrl;
            book.DescriptionUrl = modBook.DescriptionUrl;
            book.TitleUrl = modBook.TitleUrl;

            _dbContext.Books.Update(book);
            _dbContext.SaveChanges();
            return NoContent();
        }

        //----------------------------------------------
        [HttpPatch("DB/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdatePatchBookDb(int id, JsonPatchDocument<BookModel> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                return BadRequest();
            }

            var book = _dbContext.Books.AsNoTracking().FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            patchBook.ApplyTo(book, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookModel TempBook = new()
            {
                Id = book.Id,
                idBook = book.idBook,
                Title = book.Title,
                TitleUrl = book.TitleUrl,
                AuthorUrl = book.AuthorUrl,
                Author = book.Author,
                DescriptionUrl = book.DescriptionUrl,
                Description = book.Description,
                CreatedAt = book.CreatedAt
            };

            _dbContext.Books.Update(TempBook);
            _dbContext.SaveChanges();
            return NoContent();
        }

        //----------------------------------------------
        //            DTOS
        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("DbDTO/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookModel> CreateBookDbDTO([FromBody] BookModelCreateDTO newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }

            //--- Not necessary cause DTOs don't have Id
            //if (newBook.Id > 0)
            //{
            //    _logger.LogError("Error: The ID is not valid");
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            else
            {
                try
                {
                    int a = 0;
                    //if (_dbContext.Books.Count() > 0)
                    //    newBook.Id = _dbContext.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                    //else newBook.Id = 1;

                    if (_dbContext.Books.Any(book => book.Title.ToLower() == newBook.Title.ToLower()))
                    {
                        ModelState.AddModelError("SameBook", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            BookModel bookTemp = new()
            {
                idBook = newBook.idBook,
                Title = newBook.Title,
                TitleUrl = newBook.TitleUrl,
                AuthorUrl = newBook.AuthorUrl,
                Author = newBook.Author,
                DescriptionUrl = newBook.DescriptionUrl,
                Description = newBook.Description,
                CreatedAt = newBook.CreatedAt
            };

            _dbContext.Books.Add(bookTemp);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetBookDb", new { id = bookTemp.Id }, bookTemp);
        }

        //----------------------------------------------
        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPut("DbDTO/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdateBookDbDTO(int id, [FromBody] BookModelUpdateDTO modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                return BadRequest();
            }

            var book = _dbContext.Books.FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            book.idBook = modBook.idBook;
            book.Author = modBook.Author;
            book.Title = modBook.Title;
            book.Description = modBook.Description;
            book.AuthorUrl = modBook.AuthorUrl;
            book.DescriptionUrl = modBook.DescriptionUrl;
            book.TitleUrl = modBook.TitleUrl;

            _dbContext.Books.Update(book);
            _dbContext.SaveChanges();
            return NoContent();
        }

        //----------------------------------------------
        [HttpPatch("DbDTO/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdatePatchBookDbDTO(int id, JsonPatchDocument<BookModelUpdateDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                return BadRequest();
            }

            var book = _dbContext.Books.AsNoTracking().FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            BookModelUpdateDTO tempBook = new()
            {
                Id = book.Id,
                idBook = book.idBook,
                Title = book.Title,
                TitleUrl = book.TitleUrl,
                AuthorUrl = book.AuthorUrl,
                Author = book.Author,
                DescriptionUrl = book.DescriptionUrl,
                Description = book.Description,
                CreatedAt = book.CreatedAt
            };

            patchBook.ApplyTo(tempBook, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookModel bookTemp = new()
            {
                Id = tempBook.Id,
                idBook = tempBook.idBook,
                Title = tempBook.Title,
                TitleUrl = tempBook.TitleUrl,
                AuthorUrl = tempBook.AuthorUrl,
                Author = tempBook.Author,
                DescriptionUrl = tempBook.DescriptionUrl,
                Description = tempBook.Description,
                CreatedAt = tempBook.CreatedAt
            };

            _dbContext.Books.Update(bookTemp);
            _dbContext.SaveChanges();
            return NoContent();
        }

        //----------------------------------------------
        //               DataBase Async
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DbAsync/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetAllDbAsync()
        {
            _logger.LogInformation("Get all the books");
            var books = await _dbContext.Books.ToListAsync();
            //return books;
            return Ok(books);

        }
        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("DbAsync/{id:int}", Name = "GetBookDbAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookModelDTO>> GetBookDbAsync(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetBookInMem)}");
                _logger.LogError("Error: not valid ID");
                return BadRequest();
            }

            var book = new BookModel();

            try
            {
                book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.Title}");
                    return Ok(book);
                }
                else
                {
                    _logger.LogError("Error: There's no Book with this ID");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("DbAsync/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookModel>> CreateBookDbDTOAsync([FromBody] BookModelCreateDTO newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }

            //--- Not necessary cause DTOs don't have Id
            //if (newBook.Id > 0)
            //{
            //    _logger.LogError("Error: The ID is not valid");
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            else
            {
                try
                {
                    int a = 0;
                    //if (_dbContext.Books.Count() > 0)
                    //    newBook.Id = _dbContext.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                    //else newBook.Id = 1;

                    if (await _dbContext.Books.AnyAsync(book => book.Title.ToLower() == newBook.Title.ToLower()))
                    {
                        ModelState.AddModelError("SameBook", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            BookModel bookTemp = new()
            {
                idBook = newBook.idBook,
                Title = newBook.Title,
                TitleUrl = newBook.TitleUrl,
                AuthorUrl = newBook.AuthorUrl,
                Author = newBook.Author,
                DescriptionUrl = newBook.DescriptionUrl,
                Description = newBook.Description,
                CreatedAt = newBook.CreatedAt
            };

            await _dbContext.Books.AddAsync(bookTemp);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetBookDbAsync", new { id = bookTemp.Id }, bookTemp);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DbAsync/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookDbAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
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
        [HttpPut("DbAsync/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateBookDbDTOAsync(int id, [FromBody] BookModelUpdateDTO modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                return BadRequest();
            }

            var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            book.idBook = modBook.idBook;
            book.Author = modBook.Author;
            book.Title = modBook.Title;
            book.Description = modBook.Description;
            book.AuthorUrl = modBook.AuthorUrl;
            book.DescriptionUrl = modBook.DescriptionUrl;
            book.TitleUrl = modBook.TitleUrl;

            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        //----------------------------------------------
        [HttpPatch("DbAsync/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdatePatchBookDbDTOAsync(int id, JsonPatchDocument<BookModelUpdateDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                return BadRequest();
            }

            var book = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            BookModelUpdateDTO tempBook = new()
            {
                Id = book.Id,
                idBook = book.idBook,
                Title = book.Title,
                TitleUrl = book.TitleUrl,
                AuthorUrl = book.AuthorUrl,
                Author = book.Author,
                DescriptionUrl = book.DescriptionUrl,
                Description = book.Description,
                CreatedAt = book.CreatedAt
            };

            patchBook.ApplyTo(tempBook, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookModel bookTemp = new()
            {
                Id = tempBook.Id,
                idBook = tempBook.idBook,
                Title = tempBook.Title,
                TitleUrl = tempBook.TitleUrl,
                AuthorUrl = tempBook.AuthorUrl,
                Author = tempBook.Author,
                DescriptionUrl = tempBook.DescriptionUrl,
                Description = tempBook.Description,
                CreatedAt = tempBook.CreatedAt
            };

            _dbContext.Books.Update(bookTemp);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        //----------------------------------------------
        //               DataBase Automapper
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DbMap/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookModelDTO>>> GetAllDbMap()
        {
            _logger.LogInformation("Get all the books");

            IEnumerable<BookModel> bookList = await _dbContext.Books.ToListAsync();
            
            //return books;
            return Ok(_mapper.Map<IEnumerable<BookModelDTO>>(bookList));

        }


        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("DbMap/{id:int}", Name = "GetBookDbMap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookModelDTO>> GetBookDbMap(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetBookInMem)}");
                _logger.LogError("Error: not valid ID");
                return BadRequest();
            }

            var book = new BookModel();

            try
            {
                book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.Title}");
                    return Ok(_mapper.Map<BookModelDTO>(book));
                }
                else
                {
                    _logger.LogError("Error: There's no Book with this ID");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("DbMap/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookModel>> CreateBookDbDTOMap([FromBody] BookModelCreateDTO newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }

            //--- Not necessary cause DTOs don't have Id
            //if (newBook.Id > 0)
            //{
            //    _logger.LogError("Error: The ID is not valid");
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            else
            {
                try
                {
                    int a = 0;
                    //if (_dbContext.Books.Count() > 0)
                    //    newBook.Id = _dbContext.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                    //else newBook.Id = 1;

                    if (await _dbContext.Books.AnyAsync(book => book.Title.ToLower() == newBook.Title.ToLower()))
                    {
                        ModelState.AddModelError("SameBook", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            BookModel bookTemp = _mapper.Map<BookModel>(newBook);

            //BookModel bookTemp = new()
            //{
            //    idBook = newBook.idBook,
            //    Title = newBook.Title,
            //    TitleUrl = newBook.TitleUrl,
            //    AuthorUrl = newBook.AuthorUrl,
            //    Author = newBook.Author,
            //    DescriptionUrl = newBook.DescriptionUrl,
            //    Description = newBook.Description,
            //    CreatedAt = newBook.CreatedAt
            //};

            await _dbContext.Books.AddAsync(bookTemp);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetBookDb", new { id = bookTemp.Id }, bookTemp);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DbMap/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookDbMap(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
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
        [HttpPut("DbMap/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateBookDbDTOMap(int id, [FromBody] BookModelUpdateDTO modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                return BadRequest();
            }

            var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            book = _mapper.Map<BookModel>(modBook);
            
            //book.idBook = modBook.idBook;
            //book.Author = modBook.Author;
            //book.Title = modBook.Title;
            //book.Description = modBook.Description;
            //book.AuthorUrl = modBook.AuthorUrl;
            //book.DescriptionUrl = modBook.DescriptionUrl;
            //book.TitleUrl = modBook.TitleUrl;

            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        //----------------------------------------------
        [HttpPatch("DbMap/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdatePatchBookDbDTOMap(int id, JsonPatchDocument<BookModelUpdateDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                return BadRequest();
            }

            var book = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            BookModelUpdateDTO tempBook = _mapper.Map<BookModelUpdateDTO>(book);

            //BookModelUpdateDTO tempBook = new()
            //{
            //    Id = book.Id,
            //    idBook = book.idBook,
            //    Title = book.Title,
            //    TitleUrl = book.TitleUrl,
            //    AuthorUrl = book.AuthorUrl,
            //    Author = book.Author,
            //    DescriptionUrl = book.DescriptionUrl,
            //    Description = book.Description,
            //    CreatedAt = book.CreatedAt
            //};

            patchBook.ApplyTo(tempBook, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookModel bookTemp = _mapper.Map<BookModel>(tempBook);

            //BookModel bookTemp = new()
            //{
            //    Id = tempBook.Id,
            //    idBook = tempBook.idBook,
            //    Title = tempBook.Title,
            //    TitleUrl = tempBook.TitleUrl,
            //    AuthorUrl = tempBook.AuthorUrl,
            //    Author = tempBook.Author,
            //    DescriptionUrl = tempBook.DescriptionUrl,
            //    Description = tempBook.Description,
            //    CreatedAt = tempBook.CreatedAt
            //};

            _dbContext.Books.Update(bookTemp);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        //----------------------------------------------
        //               DataBase Repository
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DbRepo/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookModelDTO>>> GetAllDbRepo()
        {
            _logger.LogInformation("Get all the books");

            IEnumerable<BookModel> bookList = await _bookRepo.GetAll();

            //return books;
            return Ok(_mapper.Map<IEnumerable<BookModelDTO>>(bookList));

        }

        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("DbRepo/{id:int}", Name = "GetBookDbRepo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookModelDTO>> GetBookDbRepo(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetBookInMem)}");
                _logger.LogError("Error: not valid ID");
                return BadRequest();
            }

            var book = new BookModel();

            try
            {
                book = await _bookRepo.GetBook(book => book.Id == id);
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.Title}");
                    return Ok(_mapper.Map<BookModelDTO>(book));
                }
                else
                {
                    _logger.LogError("Error: There's no Book with this ID");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("DbRepo/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookModel>> CreateBookDbDTORepo([FromBody] BookModelCreateDTO newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }

            //--- Not necessary cause DTOs don't have Id
            //if (newBook.Id > 0)
            //{
            //    _logger.LogError("Error: The ID is not valid");
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            else
            {
                try
                {
                    int a = 0;
                    //if (_dbContext.Books.Count() > 0)
                    //    newBook.Id = _dbContext.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                    //else newBook.Id = 1;

                    if (await _bookRepo.GetBook(book => book.Title.ToLower() == newBook.Title.ToLower()) !=null)
                    {
                        ModelState.AddModelError("SameBook", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            BookModel bookTemp = _mapper.Map<BookModel>(newBook);

            //BookModel bookTemp = new()
            //{
            //    idBook = newBook.idBook,
            //    Title = newBook.Title,
            //    TitleUrl = newBook.TitleUrl,
            //    AuthorUrl = newBook.AuthorUrl,
            //    Author = newBook.Author,
            //    DescriptionUrl = newBook.DescriptionUrl,
            //    Description = newBook.Description,
            //    CreatedAt = newBook.CreatedAt
            //};

            await _bookRepo.Create(bookTemp);
            //await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetBookDb", new { id = bookTemp.Id }, bookTemp);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DbRepo/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookDbRepo(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var book = await _bookRepo.GetBook(book => book.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _bookRepo.Remove(book);
                //await _dbContext.SaveChangesAsync();
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
        [HttpPut("DbRepo/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateBookDbDTORepo(int id, [FromBody] BookModelUpdateDTO modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                return BadRequest();
            }

            var book = await _bookRepo.GetBook(book => book.Id == id, false);
            if (book == null)
            {
                return NotFound();
            }

            book = _mapper.Map<BookModel>(modBook);

            //book.idBook = modBook.idBook;
            //book.Author = modBook.Author;
            //book.Title = modBook.Title;
            //book.Description = modBook.Description;
            //book.AuthorUrl = modBook.AuthorUrl;
            //book.DescriptionUrl = modBook.DescriptionUrl;
            //book.TitleUrl = modBook.TitleUrl;

            //_dbContext.Books.Update(book);
            //await _dbContext.SaveChangesAsync();

            await _bookRepo.UpdateBook(book);

            return NoContent();
        }

        //----------------------------------------------
        [HttpPatch("DbRepo/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdatePatchBookDbDTORepo(int id, JsonPatchDocument<BookModelUpdateDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                return BadRequest();
            }

            //var book = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);
            var book = await _bookRepo.GetBook(book => book.Id == id, false);

            if (book == null)
            {
                return NotFound();
            }

            BookModelUpdateDTO tempBook = _mapper.Map<BookModelUpdateDTO>(book);

            //BookModelUpdateDTO tempBook = new()
            //{
            //    Id = book.Id,
            //    idBook = book.idBook,
            //    Title = book.Title,
            //    TitleUrl = book.TitleUrl,
            //    AuthorUrl = book.AuthorUrl,
            //    Author = book.Author,
            //    DescriptionUrl = book.DescriptionUrl,
            //    Description = book.Description,
            //    CreatedAt = book.CreatedAt
            //};

            patchBook.ApplyTo(tempBook, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookModel bookTemp = _mapper.Map<BookModel>(tempBook);

            //BookModel bookTemp = new()
            //{
            //    Id = tempBook.Id,
            //    idBook = tempBook.idBook,
            //    Title = tempBook.Title,
            //    TitleUrl = tempBook.TitleUrl,
            //    AuthorUrl = tempBook.AuthorUrl,
            //    Author = tempBook.Author,
            //    DescriptionUrl = tempBook.DescriptionUrl,
            //    Description = tempBook.Description,
            //    CreatedAt = tempBook.CreatedAt
            //};

            //_dbContext.Books.Update(bookTemp);
            //await _dbContext.SaveChangesAsync();

            await _bookRepo.UpdateBook(bookTemp);

            return NoContent();
        }

        //----------------------------------------------
        //               DataBase Repository APIResponse
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DbAPIResponse/")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllDbAPIResponse()
        {
            try
            {
                _logger.LogInformation("Get all the books");

                IEnumerable<BookModel> bookList = await _bookRepo.GetAll();

                _response.Result = _mapper.Map<IEnumerable<BookModelDTO>>(bookList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.Message
                };

                return _response;
                
            }


        }

        //----------------------------------------------
        /// <summary>
        /// Get the book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the book that match with the ID passed </returns>
        [HttpGet("DbAPIResponse/{id:int}", Name = "GetBookDbAPIResponse")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBookDbPIResponse(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetBookInMem)}");
                _logger.LogError("Error: not valid ID");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var book = new BookModel();

            try
            {
                book = await _bookRepo.GetBook(book => book.Id == id);
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.Title}");
                    _response.IsSuccess = true;
                    _response.Result = _mapper.Map<BookModelDTO>(book);
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                else
                {
                    _logger.LogError("Error: There's no Book with this ID");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
                return BadRequest(_response);
            }
        }

        //----------------------------------------------
        /// <summary>
        /// Create a new Book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPost("DbAPIResponse/")]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateBookDbDTOAPIResponse([FromBody] BookModelCreateDTO newBook)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Error:" + ModelState.Values);
                return BadRequest(ModelState);
            }

            if (newBook == null)
            {
                _logger.LogError("Error: Empty Book");
                return BadRequest(newBook);
            }

            //--- Not necessary cause DTOs don't have Id
            //if (newBook.Id > 0)
            //{
            //    _logger.LogError("Error: The ID is not valid");
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            else
            {
                try
                {
                    int a = 0;
                    //if (_dbContext.Books.Count() > 0)
                    //    newBook.Id = _dbContext.Books.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                    //else newBook.Id = 1;

                    if (await _bookRepo.GetBook(book => book.Title.ToLower() == newBook.Title.ToLower()) != null)
                    {
                        ModelState.AddModelError("ErrorMessage", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            BookModel bookTemp = _mapper.Map<BookModel>(newBook);

            //BookModel bookTemp = new()
            //{
            //    idBook = newBook.idBook,
            //    Title = newBook.Title,
            //    TitleUrl = newBook.TitleUrl,
            //    AuthorUrl = newBook.AuthorUrl,
            //    Author = newBook.Author,
            //    DescriptionUrl = newBook.DescriptionUrl,
            //    Description = newBook.Description,
            //    CreatedAt = newBook.CreatedAt
            //};

            await _bookRepo.Create(bookTemp);
            _response.Result = bookTemp;
            _response.StatusCode = HttpStatusCode.Created;
            _response.IsSuccess = true;
            //await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetBookDb", new { id = bookTemp.Id }, _response);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DbAPIResponse/{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookDbAPIResponse(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var book = await _bookRepo.GetBook(book => book.Id == id);

            if (book == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound; 
                return NotFound(_response);
            }
            else
            {
                await _bookRepo.Remove(book);
                //await _dbContext.SaveChangesAsync();
                _response.StatusCode=HttpStatusCode.NoContent;
                _response.IsSuccess = true;
            }
            return Ok(_response);
        }

        //----------------------------------------------
        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modBook"></param>
        /// <returns>The result of the operation</returns>
        [HttpPut("DbAPIResponse/{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateBookDbDTOAPIResponse(int id, [FromBody] BookModelUpdateDTO modBook)
        {
            if (modBook == null || id != modBook.Id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest; 
                return BadRequest(_response);
            }

            var book = await _bookRepo.GetBook(book => book.Id == id, false);
            if (book == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            book = _mapper.Map<BookModel>(modBook);

            //book.idBook = modBook.idBook;
            //book.Author = modBook.Author;
            //book.Title = modBook.Title;
            //book.Description = modBook.Description;
            //book.AuthorUrl = modBook.AuthorUrl;
            //book.DescriptionUrl = modBook.DescriptionUrl;
            //book.TitleUrl = modBook.TitleUrl;

            //_dbContext.Books.Update(book);
            //await _dbContext.SaveChangesAsync();

            await _bookRepo.UpdateBook(book);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess=true;

            return Ok(_response);
        }

        //----------------------------------------------
        [HttpPatch("DbAPIResponse/{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdatePatchBookDbDTOAPIResponse(int id, JsonPatchDocument<BookModelUpdateDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest();
            }

            //var book = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);
            var book = await _bookRepo.GetBook(book => book.Id == id, false);

            if (book == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound();
            }

            BookModelUpdateDTO tempBook = _mapper.Map<BookModelUpdateDTO>(book);

            //BookModelUpdateDTO tempBook = new()
            //{
            //    Id = book.Id,
            //    idBook = book.idBook,
            //    Title = book.Title,
            //    TitleUrl = book.TitleUrl,
            //    AuthorUrl = book.AuthorUrl,
            //    Author = book.Author,
            //    DescriptionUrl = book.DescriptionUrl,
            //    Description = book.Description,
            //    CreatedAt = book.CreatedAt
            //};

            patchBook.ApplyTo(tempBook, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookModel bookTemp = _mapper.Map<BookModel>(tempBook);

            //BookModel bookTemp = new()
            //{
            //    Id = tempBook.Id,
            //    idBook = tempBook.idBook,
            //    Title = tempBook.Title,
            //    TitleUrl = tempBook.TitleUrl,
            //    AuthorUrl = tempBook.AuthorUrl,
            //    Author = tempBook.Author,
            //    DescriptionUrl = tempBook.DescriptionUrl,
            //    Description = tempBook.Description,
            //    CreatedAt = tempBook.CreatedAt
            //};

            //_dbContext.Books.Update(bookTemp);
            //await _dbContext.SaveChangesAsync();

            await _bookRepo.UpdateBook(bookTemp);
            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
    }
}
