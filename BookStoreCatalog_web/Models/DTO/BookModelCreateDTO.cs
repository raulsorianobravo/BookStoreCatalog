using System.ComponentModel.DataAnnotations;

namespace BookStoreCatalog_web.Models.DTO
{
    public class BookModelCreateDTO
    {
        #region PROPERTIES

        //--- Book id
        //public int Id { get; set; }

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
