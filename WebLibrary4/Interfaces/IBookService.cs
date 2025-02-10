using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Books>> GetAllBooksAsync();
    Task<Books?> GetBookByIdAsync(int id);
    Task<int> AddBookAsync(Books book);
    Task<bool> UpdateBookDescriptionAsync(int id, BookUpdateDiscrptionDto dto);
    Task<bool> DeleteBookAsync(int id);
    Task<int?> AddPdfToBookAsync(PdfDocument pdf, int bookId);
    Task<PdfDocument> ShowContetnBook(int id);
}