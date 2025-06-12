using AutoMapper;
using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalog_web.ViewModel;
using BookStoreCatalogUtils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BookStoreCatalog_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;


        public HomeController(ILogger<HomeController> logger, IBookService bookService, IMapper mapper)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            List<BookModelDTO> bookList = new List<BookModelDTO>();

            if (pageNumber < 1) 
            {
                pageNumber = 1;
            }

            BookPagedViewModel bookPagedViewModel = new BookPagedViewModel();

            var response = await _bookService.GetAllBooksPaged<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken),pageNumber, 3);

            if (response != null && response.IsSuccess)
            {
                bookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(response.Result));
                bookPagedViewModel = new BookPagedViewModel()
                {
                    BookList = bookList,
                    PageNumber = pageNumber,
                    PageSize = JsonConvert.DeserializeObject<int>(Convert.ToString(response.TotalPages))
                };

                if (pageNumber > 1) 
                {
                    bookPagedViewModel.Prev = "";
                }
                if (bookPagedViewModel.PageSize <= pageNumber)
                {
                    bookPagedViewModel.Next = "disabled";
                }

            }

            return View(bookPagedViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
