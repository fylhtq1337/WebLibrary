using AutoMapper;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Mapping;

 
public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
         
        CreateMap<Books, BookResponseDto>()
            .ForMember(dest => dest.BorrowRecords, opt => opt.MapFrom(src => src.BorrowRecords));

        
        CreateMap<BookRequestDto, Books>();
        CreateMap<Books, BookShortDto>();
    }
}