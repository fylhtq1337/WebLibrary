using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Books>> GetAllBooksAsync();
    Task<Books?> GetBookByIdAsync(int id);
    Task<int> AddBookAsync(Books book);
    Task<bool> UpdateBookAsync(Books book);
    Task<bool> DeleteBookAsync(int id);
    Task<int?> AddPdfToBookAsync(PdfDocument pdf, int bookId);
}