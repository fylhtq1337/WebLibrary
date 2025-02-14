using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs;
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
        
        public async Task<IEnumerable<Books>> SearchBooksAsync(string? title )
        {
             
            title = string.IsNullOrWhiteSpace(title) ? null : title;
            

             
            var books = await _bookRepository.SearchBooksAsync(title);

            return books;  
        }
        
        public async Task<int> GetTotalBookCountAsync()
        {
            return await _bookRepository.GetTotalBookCountAsync();
        }

        public async Task<IEnumerable<Books>> GetBooksPaginatedAsync(int page, int pageSize)
        {
            return await _bookRepository.GetBooksPaginatedAsync(page, pageSize);
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
        
        public async Task<bool> UpdateBookDescriptionAsync(int id, BookUpdateDiscrptionDto dto)
        {
             
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Описание книги не может быть пустым.");
            }

             
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                 
                return false;
            }

             
            existingBook.Description = dto.Description;

            
            await _bookRepository.UpdateAsync(existingBook);

             
            return true;
        }
        
         

        public async Task<bool> DeleteBookAsync(int id)
        {
             
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                
                return false;
            }

             
            await _bookRepository.DeleteAsync(existingBook.Id);
            return true;  
        }
    }
}