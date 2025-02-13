using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Books>> GetAllAsync();
    Task<IEnumerable<Books>> SearchBooksAsync(string? title);
    // Получить общее количество записей книг
    Task<int> GetTotalBookCountAsync();

    // Получить книги с учетом пагинации
    Task<IEnumerable<Books>> GetBooksPaginatedAsync(int page, int pageSize);

    
    Task<Books?> GetByIdAsync(int id);  
    Task<int> AddAsync(Books book);  
    Task UpdateAsync(Books book);  
    Task DeleteAsync(int id);  
    Task<int> AddPdfAsync(PdfDocument pdf, int bookId);
    Task<PdfDocument> ReturnPdf(int id);
}