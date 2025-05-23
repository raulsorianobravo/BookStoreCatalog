using AutoMapper;
using BookStoreCatalog_web.Models.DTO;

namespace BookStoreCatalog_web
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<BookModelDTO, BookModelCreateDTO>();
            CreateMap<BookModelCreateDTO, BookModelDTO>();


            CreateMap<BookModelDTO, BookModelUpdateDTO>().ReverseMap();

            //-------------------------------------------------

            CreateMap<IssueModelDTO, IssueModelCreateDTO>().ReverseMap();
            CreateMap<IssueModelDTO, IsssueModelUpdateDTO>().ReverseMap();

        }
    }
}
