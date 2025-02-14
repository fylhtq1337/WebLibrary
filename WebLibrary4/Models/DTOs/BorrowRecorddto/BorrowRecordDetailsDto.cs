using WebLibrary4.Models.DTOs.Clientsdto;

namespace WebLibrary4.Models.DTOs.BorrowRecorddto;

public class BorrowRecordDetailsDto
{
    public int Id { get; set; }

    public int BookId { get; set; }
 
    public int UserId { get; set; }
 
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public ClientDto? Borrower { get; set; }
    public BookRequestDto? BorrowedBook { get; set; }  
}