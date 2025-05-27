using AutoMapper;
using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalog_web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BookStoreCatalog_web.Controllers
{
    public class IssueController : Controller
    {

        private readonly IIssueService _issueService;
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;

        public IssueController(IIssueService issueService, IMapper mapper, IBookService bookService)
        {
            _issueService = issueService;
            _mapper = mapper;
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexIssue()
        {
            List<IssueModelDTO> issueList = new List<IssueModelDTO>();

            var response = await _issueService.GetAllIssues<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                issueList = JsonConvert.DeserializeObject<List<IssueModelDTO>>(Convert.ToString(response.Result));
            }

            return View(issueList);
        }

        public async Task<IActionResult> CreateIssue()
        {
            
            IssueViewModel issueViewModel = new IssueViewModel();
            var response = await _bookService.GetAllBooks<APIResponse>();

            if(response != null && response.IsSuccess)
            {
                issueViewModel.BookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(response.Result))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Title,
                        Value = v.Id.ToString()
                    });
            }
            
            return View(issueViewModel);
        }
    }
}
