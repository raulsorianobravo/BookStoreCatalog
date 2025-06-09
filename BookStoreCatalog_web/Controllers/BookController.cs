using AutoMapper;
using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalogUtils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookStoreCatalog_web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexBook()
        {
            List<BookModelDTO> bookList = new List<BookModelDTO>();

            var response = await _bookService.GetAllBooks<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                bookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(response.Result));
            }

            return View(bookList);
        }

        public async Task<IActionResult> CreateNewBook()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewBook(BookModelCreateDTO book)
        {
            if (ModelState.IsValid) 
            {
                var response = await _bookService.CreateBook<APIResponse>(book, HttpContext.Session.GetString(ClassDefinitions.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Book Saved";
                    return RedirectToAction(nameof(IndexBook));
                }
            }
            return View(book);
        }

        
        public async Task<IActionResult> UpdateBook(int Id)
        {
            var response = await _bookService.GetBook<APIResponse>(Id, HttpContext.Session.GetString(ClassDefinitions.SessionToken));

            if (response != null && response.IsSuccess)
            {
                BookModelDTO bookDTO = JsonConvert.DeserializeObject<BookModelDTO>(Convert.ToString(response.Result));
                BookModelUpdateDTO book = _mapper.Map<BookModelUpdateDTO>(bookDTO);
                return View(book);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBook(BookModelUpdateDTO book)
        {
            if (ModelState.IsValid)
            {
                var response = await _bookService.UpdateBook<APIResponse>(book, HttpContext.Session.GetString(ClassDefinitions.SessionToken));

                if(response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Book Updated";
                    return RedirectToAction(nameof(IndexBook));
                }
            }
            return View(book);

        }

        public async Task<IActionResult> DeleteBook(int Id)
        {
            var response = await _bookService.GetBook<APIResponse>(Id, HttpContext.Session.GetString(ClassDefinitions.SessionToken));

            if (response != null && response.IsSuccess)
            {
                BookModelDTO bookDTO = JsonConvert.DeserializeObject<BookModelDTO>(Convert.ToString(response.Result));
                return View(bookDTO);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(BookModelDTO book)
        {

            var response = await _bookService.DeleteBook<APIResponse>(book.Id, HttpContext.Session.GetString(ClassDefinitions.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book Deleted";
                return RedirectToAction(nameof(IndexBook));
            }
            TempData["Error"] = "Something goes wrong";
            return View(book);

        }
    }
}
