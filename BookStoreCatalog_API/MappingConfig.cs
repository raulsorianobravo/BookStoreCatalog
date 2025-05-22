using AutoMapper;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;

namespace BookStoreCatalog_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<BookModel, BookModelDTO>();
            CreateMap<BookModelDTO, BookModel>();

            CreateMap<BookModel, BookModelCreateDTO>();
            CreateMap<BookModelCreateDTO, BookModel>();

            CreateMap<BookModel, BookModelUpdateDTO>().ReverseMap();

            //-------------------------------------------------

            CreateMap<IssueModel, IssueModelDTO>().ReverseMap();
            CreateMap<IssueModel, IssueModelCreateDTO>().ReverseMap();
            CreateMap<IssueModel, IsssueModelUpdateDTO>().ReverseMap();

        }

    }
}
