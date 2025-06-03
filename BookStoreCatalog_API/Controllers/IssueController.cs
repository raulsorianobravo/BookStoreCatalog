using Azure;
using BookStoreCatalog_API.Models.DTO;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;
using BookStoreCatalog_API.Data;
using Microsoft.AspNetCore.JsonPatch;

namespace BookStoreCatalog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        //----------------------------------------------
        //--- Logger
        private readonly ILogger<IssueController> _logger;

        //--- DbContext
        private readonly ApplicationDBContextInMem _context;

        //--- DbContext SQL Server
        private readonly ApplicationDbContext _dbContext;

        private readonly IMapper _mapper;

        private readonly IBookRepo _bookRepo;


        private readonly IIssueRepo _issueRepo;

        protected APIResponse _response;


        //----------------------------------------------

        /// <summary>
        /// Constructor - Injection
        /// </summary>
        public IssueController(ILogger<IssueController> logger, ApplicationDBContextInMem context, ApplicationDbContext dbContext, IMapper mapper, IBookRepo bookRepo, IIssueRepo issueRepo)
        {
            _logger = logger;
            _context = context;
            _context.Database.EnsureCreated();
            _dbContext = dbContext;
            _mapper = mapper;
            _bookRepo = bookRepo;
            _issueRepo = issueRepo;

            _response = new APIResponse();
        }

        //----------------------------------------------
        //               DataBase Repository APIResponse
        //----------------------------------------------
        /// <summary>
        /// Get all the Books
        /// </summary>
        /// <returns> A fake list of Books </returns>
        [HttpGet("DbAPIResponse/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllDbAPIResponse()
        {
            try
            {
                _logger.LogInformation("Get all the books");

                IEnumerable<IssueModel> issueList = await _issueRepo.GetAll(includeProperties:"Book");

                _response.Result = _mapper.Map<IEnumerable<IssueModelDTO>>(issueList);
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
        [HttpGet("DbAPIResponse/{id:int}", Name = "GetIssueDbAPIResponse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetIssueDbAPIResponse(int id)
        {
            if (id == 0)
            {
                _logger.LogInformation($"{nameof(GetIssueDbAPIResponse)}");
                _logger.LogError("Error: not valid ID");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var book = new IssueModel();

            try
            {
                book = await _issueRepo.GetBook(book => book.IssueId == id, includeProperties:"Book");
                if (book != null)
                {
                    _logger.LogInformation("Sucessful:" + $"{book.IssueName}");
                    _response.IsSuccess = true;
                    _response.Result = _mapper.Map<IssueModelDTO>(book);
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateBookDbDTOAPIResponse([FromBody] IssueModelCreateDTO newBook)
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

                    if (await _issueRepo.GetBook(book => book.IssueName.ToLower() == newBook.IssueName.ToLower()) != null)
                    {
                        ModelState.AddModelError("ErrorMessage", "This Book Exists, don't insist");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }

                    if (await _bookRepo.GetBook(v => v.Id == newBook.BookId) == null)
                    {
                        ModelState.AddModelError("ErrorMessage", "Can't find this book");
                        _logger.LogError("Error:" + ModelState.ToList()[0].Value.Errors[0].ErrorMessage);
                        return BadRequest(ModelState);
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }

            IssueModel bookTemp = _mapper.Map<IssueModel>(newBook);

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

            await _issueRepo.Create(bookTemp);
            _response.Result = bookTemp;
            _response.StatusCode = HttpStatusCode.Created;
            _response.IsSuccess = true;
            //await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetIssueDbAPIResponse", new { id = bookTemp.IssueId }, _response);
        }

        //----------------------------------------------
        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DbAPIResponse/{id:int}")]
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
            var book = await _issueRepo.GetBook(book => book.IssueId == id);

            if (book == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            else
            {
                await _issueRepo.Remove(book);
                //await _dbContext.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateBookDbDTOAPIResponse(int id, [FromBody] IsssueModelUpdateDTO modBook)
        {
            if (modBook == null || id != modBook.IssueId)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var book = await _issueRepo.GetBook(book => book.IssueId == id, false);
            if (book == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            book = _mapper.Map<IssueModel>(modBook);

            //book.idBook = modBook.idBook;
            //book.Author = modBook.Author;
            //book.Title = modBook.Title;
            //book.Description = modBook.Description;
            //book.AuthorUrl = modBook.AuthorUrl;
            //book.DescriptionUrl = modBook.DescriptionUrl;
            //book.TitleUrl = modBook.TitleUrl;

            //_dbContext.Books.Update(book);
            //await _dbContext.SaveChangesAsync();

            await _issueRepo.UpdateBook(book);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.NoContent;


            return Ok(_response);
        }

        //----------------------------------------------
        [HttpPatch("DbAPIResponse/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdatePatchBookDbDTOAPIResponse(int id, JsonPatchDocument<IsssueModelUpdateDTO> patchBook)
        {
            if (patchBook == null || id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest();
            }

            //var book = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);
            var book = await _issueRepo.GetBook(book => book.IssueId == id, false);

            if (book == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound();
            }

            IsssueModelUpdateDTO tempBook = _mapper.Map<IsssueModelUpdateDTO>(book);

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

            IssueModel bookTemp = _mapper.Map<IssueModel>(tempBook);

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

            await _issueRepo.UpdateBook(bookTemp);
            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }

    }
}
