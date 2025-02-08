using WebLibrary4.Models.DTOs.BorrowRecorddto;

namespace WebLibrary4.Models.DTOs;

public class BookResponseDto
{
    public int Id { get; set; }  
    public string Title { get; set; } = string.Empty;  
    public string Description { get; set; } = string.Empty;  
    public string Author { get; set; } = string.Empty;  
    public string Genre { get; set; } = string.Empty;  
    public int Year { get; set; }  
    public int Amount { get; set; }  
    public List<BorrowRecordDto>? BorrowRecords { get; set; }
}