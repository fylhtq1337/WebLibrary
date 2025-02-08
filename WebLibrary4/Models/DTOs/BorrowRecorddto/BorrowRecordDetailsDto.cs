using WebLibrary4.Models.DTOs.Clientsdto;

namespace WebLibrary4.Models.DTOs.BorrowRecorddto;

public class BorrowRecordDetailsDto
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public string? BookTitle { get; set; }

    public int UserId { get; set; }
    public string? UserName { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    // Дополнительные данные о пользователе или книге
    public ClientDto? Borrower { get; set; }
    public BookRequestDto? BorrowedBook { get; set; }  
}