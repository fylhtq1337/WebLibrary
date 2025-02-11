using WebLibrary4.Models.DTOs.BorrowRecorddto;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces;

public interface IBorrowRecordRepository
{
    Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> SearchBorrowRecordsAsync(string? bookTitle,
        string? clientName);
    Task<IEnumerable<BorrowRecord>> GetAllAsync();
    Task<BorrowRecord?> GetByIdAsync(int id);
    Task<int> AddAsync(BorrowRecord borrowRecord);
    Task UpdateAsync(BorrowRecord borrowRecord);
    Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> GetDetailedBorrowRecordsAsync();
    Task<bool> DeleteAsync(int id);
}