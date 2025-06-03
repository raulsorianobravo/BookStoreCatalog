using BookStoreCatalog_web.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreCatalog_web.ViewModel
{
    public class IssueUpdateViewModel
    {
        public IsssueModelUpdateDTO Issue { get; set; }
        public IEnumerable<SelectListItem> BookList { get; set; }

        public IssueUpdateViewModel()
        {
            Issue = new IsssueModelUpdateDTO();
        }
    }
}
