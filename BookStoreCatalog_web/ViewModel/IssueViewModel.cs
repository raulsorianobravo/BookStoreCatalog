using BookStoreCatalog_web.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreCatalog_web.ViewModel
{
    public class IssueViewModel
    {
        public IssueModelCreateDTO Issue { get; set; }
        public IEnumerable<SelectListItem> BookList { get; set; }

        public IssueViewModel()
        {
            Issue = new IssueModelCreateDTO();
        }
    }
}
