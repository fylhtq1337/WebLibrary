using WebLibrary4.Models.DTOs.BorrowRecorddto;

namespace WebLibrary4.Interfaces
{
    public interface IBorrowRecordService
    {
        Task<IEnumerable<BorrowRecordDto>> GetAllAsync(); // Получение всех записей
        Task<BorrowRecordDetailsDto?> GetByIdAsync(int id); // Получение записи по Id
        Task<int> CreateBorrowAsync(BorrowRecordDto borrowRecordDto); // Добавление новой записи
        Task UpdateAsync(BorrowRecordDto borrowRecordDto); // Обновление записи
        Task<bool> DeleteAsync(int id); // Удаление записи
    }
}