using AutoMapper;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Mapping;

// Профиль AutoMapper для Books
public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        // Books -> BookResponseDto
        CreateMap<Books, BookResponseDto>()
            .ForMember(dest => dest.BorrowRecords, opt => opt.MapFrom(src => src.BorrowRecords));

        // BookRequestDto -> Books
        CreateMap<BookRequestDto, Books>();
    }
}