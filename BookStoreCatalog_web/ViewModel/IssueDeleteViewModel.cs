using BookStoreCatalog_web.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreCatalog_web.ViewModel
{
    public class IssueDeleteViewModel
    {

        public IssueModelDTO Issue { get; set; }
        public IEnumerable<SelectListItem> BookList { get; set; }

        public IssueDeleteViewModel()
        {
            Issue = new IssueModelDTO();
        }
    }
}
