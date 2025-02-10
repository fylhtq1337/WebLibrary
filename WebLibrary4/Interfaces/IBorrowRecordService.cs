using WebLibrary4.Models.DTOs.BorrowRecorddto;

namespace WebLibrary4.Interfaces
{
    public interface IBorrowRecordService
    {
        Task<IEnumerable<BorrowRecordDto>> GetAllAsync();  
        Task<BorrowRecordDetailsDto?> GetByIdAsync(int id);  
        Task<int> CreateBorrowAsync(BorrowRecordDto borrowRecordDto);  
        Task UpdateAsync(BorrowRecordDto borrowRecordDto);  
        Task<bool> DeleteAsync(int id);  

        Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> GetDetailedBorrowRecordsAsync();
    }
}