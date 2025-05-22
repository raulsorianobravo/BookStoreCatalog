using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreCatalog_API.Models
{
    public class IssueModel
    {

        [Key]
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IssueId { get; set; }

        [Required]
        public int IssueNumber { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public BookModel Book { get; set; } 

        public string IssueName { get; set; }

        public string IssueDescription { get; set; }

        public string extraInfo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        
        



    }
}
