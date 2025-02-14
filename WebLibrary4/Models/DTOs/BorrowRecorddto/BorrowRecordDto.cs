namespace WebLibrary4.Models.DTOs.BorrowRecorddto;

public class BorrowRecordDto
{
    public int Id { get; set; }  
    public int BookId { get; set; }  
    public int ClientId { get; set; }  
    public DateTime BorrowDate { get; set; }  
    public DateTime? ReturnDate { get; set; }  
}