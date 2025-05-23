using System.ComponentModel.DataAnnotations;

namespace BookStoreCatalog_web.Models.DTO
{
    public class IssueModelCreateDTO
    {
        [Required]
        public int IssueNumber { get; set; }

        [Required]
        public int BookId { get; set; }

        public string IssueName { get; set; }

        public string IssueDescription { get; set; }

        public string extraInfo { get; set; }
    }
}
