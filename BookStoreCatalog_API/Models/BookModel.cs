using System.ComponentModel.DataAnnotations;

namespace BookStoreCatalog_API.Models
{
    /**
   * CLASS
   *
   * Book element Model Class
   *
   * @param 
   * @property
   *
   * @constructor N/A
   *
   * @author Raul Soriano Bravo
   * @since 2025-05-01
   *
   */
    
    public class BookModel
    {
        #region PROPERTIES

        //--- Book id
        [Key]
        public int Id { get; set; }

        //--- Internal ID
        public string idBook { get; set; }

        //--- Book Title         
        [Required]
        [MaxLength(50)]
        public string Title { get; set; } 
        
        //--- Book Description
        public string Description { get; set; }
        
        //--- Author
        public string Author { get; set; }
        
        //--- Authors Website
        public string AuthorUrl { get; set; }
        
        //--- Title Website
        public string TitleUrl { get; set; }
        
        //--- Description 
        public string DescriptionUrl { get; set; }

        //--- Date created
        public DateTime CreatedAt { get; set; }
        #endregion
    }
}
