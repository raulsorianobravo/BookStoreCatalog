using AutoMapper;
using BookStoreCatalog_web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

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
    }
}
