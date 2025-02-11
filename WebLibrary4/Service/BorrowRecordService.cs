using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.DTOs.BorrowRecorddto;
using WebLibrary4.Models.DTOs.Clientsdto;
using WebLibrary4.Models.Entities;
using WebLibrary4.Repositories;

namespace WebLibrary4.Services
{
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly IBorrowRecordRepository _repository; // Репозиторий

        public BorrowRecordService(IBorrowRecordRepository repository)
        {
            _repository = repository;
        }
        public async  Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> SearchByBookTitle(string bookTitle)
        {
            var results = await _repository.SearchByBookTitleAsync(bookTitle);
            return  results;
        }

        public async  Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> SearchByClientName(string clientName)
        {
            var results = await _repository.SearchByClientNameAsync(clientName);
            return results;
        }

        // Получение всех записей
        public async Task<IEnumerable<BorrowRecordDto>> GetAllAsync()
        {
            var borrowRecords = await _repository.GetAllAsync();
            
            // Преобразуем сущности в DTO
            return borrowRecords.Select(record => new BorrowRecordDto
            {
                Id = record.Id,
                BookId = record.BookId,
                ClientId = record.ClientId,
                BorrowDate = record.BorrowDate,
                ReturnDate = record.ReturnDate
            });
        }
        
        public async Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> GetDetailedBorrowRecordsAsync()
        {
            // Вызываем метод репозитория для получения данных
            var detailedRecords = await _repository.GetDetailedBorrowRecordsAsync();

            // Преобразуем данные из репозитория (если необходимо, тут уже совпадают DTO и возвращаемые поля)
            return detailedRecords.Select(record => new BorrowRecordClientNameBookTitleDto
            {
                Id = record.Id,
                ClientName = record.ClientName,
                BookTitle = record.BookTitle,
                BorrowDate = record.BorrowDate,
                ReturnDate = record.ReturnDate
            });
        }

        //  этот метод нужно в  доработать 
        public async Task<BorrowRecordDetailsDto?> GetByIdAsync(int id)
        {
            var borrowRecord = await _repository.GetByIdAsync(id);

            if (borrowRecord == null)
            {
                return null; // Если запись не найдена, возвращаем null
            }

            // Преобразуем сущность в Detailed DTO
            return new BorrowRecordDetailsDto
            {
                Id = borrowRecord.Id,
                BookId = borrowRecord.BookId,
                UserId = borrowRecord.ClientId,
                BorrowDate = borrowRecord.BorrowDate,
                ReturnDate = borrowRecord.ReturnDate,
                Borrower = borrowRecord.Borrower != null ? new ClientDto
                {
                    Id = borrowRecord.Borrower.Id,
                    Username = borrowRecord.Borrower.Username,
                    Email = borrowRecord.Borrower.Email
                } : null,
                BorrowedBook = borrowRecord.BorrowedBook != null ? new BookRequestDto
                {
                     Title = borrowRecord.BorrowedBook.Title,
                    Author = borrowRecord.BorrowedBook.Author
                } : null
            };
        }

        // Добавление записи
        public async Task<int> CreateBorrowAsync(BorrowRecordDto borrowRecordDto)
        {
            // Преобразуем DTO в сущность
            var borrowRecord = new BorrowRecord
            {
                BookId = borrowRecordDto.BookId,
                ClientId = borrowRecordDto.ClientId,
                BorrowDate = borrowRecordDto.BorrowDate,
                ReturnDate = borrowRecordDto.ReturnDate
            };

            // Вызываем метод репозитория
            return await _repository.AddAsync(borrowRecord);
        }

        // Обновление записи
        public async Task UpdateAsync(BorrowRecordDto borrowRecordDto)
        {
            // Преобразуем DTO в сущность
            var borrowRecord = new BorrowRecord
            {
                Id = borrowRecordDto.Id,
                BookId = borrowRecordDto.BookId,
                ClientId = borrowRecordDto.ClientId,
                BorrowDate = borrowRecordDto.BorrowDate,
                ReturnDate = borrowRecordDto.ReturnDate
            };

            // Вызываем метод репозитория
            await _repository.UpdateAsync(borrowRecord);
        }
        
        public async Task<bool> MarkAsReturnedAsync(int id)
        {
            var record = await _repository.GetByIdAsync(id);

            if (record == null)
                return false; // Запись не найдена

            record.ReturnDate = DateTime.UtcNow; // Устанавливаем дату возврата (текущая)
            await _repository.UpdateAsync(record);

            return true;
        }

        // Удаление записи
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}