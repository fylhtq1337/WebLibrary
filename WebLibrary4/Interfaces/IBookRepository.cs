using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Books>> GetAllAsync();  
    Task<Books?> GetByIdAsync(int id);  
    Task<int> AddAsync(Books book);  
    Task UpdateAsync(Books book);  
    Task DeleteAsync(int id);  
    Task<int> AddPdfAsync(PdfDocument pdf, int bookId);
    Task<PdfDocument> ReturnPdf(int id);
}