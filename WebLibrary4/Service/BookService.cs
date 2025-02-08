using WebLibrary4.Interfaces;
using WebLibrary4.Models.Entities;
 

namespace WebLibrary4.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Books>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Books?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<int> AddBookAsync(Books book)
        {
            return await _bookRepository.AddAsync(book);
        }

        public async Task<bool> UpdateBookAsync(Books book)
        {
            // Пробуем выполнить обновление через репозиторий
            var existingBook = await _bookRepository.GetByIdAsync(book.Id);
            if (existingBook == null)
            {
                // Если книга с переданным Id не найдена, возвращаем false
                return false;
            }

            // Если книга существует, обновляем её
            await _bookRepository.UpdateAsync(book);
            return true; // Обновление успешно
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            // Проверяем, существует ли книга
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                // Если книги нет, возвращаем false
                return false;
            }

            // Если книга существует, удаляем её
            await _bookRepository.DeleteAsync(existingBook.Id);
            return true; // Успешно удалено
        }
    }
}