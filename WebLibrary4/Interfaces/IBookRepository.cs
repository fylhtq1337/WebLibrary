using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Books>> GetAllAsync(); // Получить все книги из базы
    Task<Books?> GetByIdAsync(int id); // Получить книгу по ID
    Task<int> AddAsync(Books book); // Добавить новую книгу
    Task UpdateAsync(Books book); // Обновить книгу
    Task DeleteAsync(int id); // Удалить книгу по ID
}