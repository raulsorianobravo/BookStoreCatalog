using AutoMapper;
using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services;
using BookStoreCatalog_web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookStoreCatalog_web.Controllers
{
    public class IssueController : Controller
    {

        private readonly IIssueService _issueService;
        private readonly IMapper _mapper;

        public IssueController(IIssueService issueService, IMapper mapper)
        {
            _issueService = issueService;
            _mapper = mapper;
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
    }
}
