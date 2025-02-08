using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Books>> GetAllBooksAsync();
    Task<Books?> GetBookByIdAsync(int id);
    Task<int> AddBookAsync(Books book);
    Task UpdateBookAsync(Books book);
    Task DeleteBookAsync(int id);
}