using AutoMapper;
using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookStoreCatalog_web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _booService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _booService = bookService;
            _mapper = mapper;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexBook()
        {
            List<BookModelDTO> bookList = new List<BookModelDTO>();

            var response = await _booService.GetAllBooks<APIResponse>();
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
                var response = await _booService.CreateBook<APIResponse>(book);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexBook));
                }
            }
            return View(book);
        }
    }
}
