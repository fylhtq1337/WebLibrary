namespace WebLibrary4.Models.DTOs;

public class BorrowRecordResponseDto
{
    public int Id { get; set; }  
    public DateTime BorrowDate { get; set; }  
    public DateTime? ReturnDate { get; set; }  
    public int BookId { get; set; }  
    public string BorrowedBy { get; set; } = string.Empty;  
}