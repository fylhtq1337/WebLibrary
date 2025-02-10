namespace WebLibrary4.Models.DTOs.BorrowRecorddto;

public class BorrowRecordClientNameBookTitleDto
{
    public string ClientName { get; set; } = string.Empty;  
    public string BookTitle { get; set; } = string.Empty;   
    public DateTime BorrowDate { get; set; }               
    public DateTime? ReturnDate { get; set; }    
}