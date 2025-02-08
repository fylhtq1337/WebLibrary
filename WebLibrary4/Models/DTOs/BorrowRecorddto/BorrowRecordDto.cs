namespace WebLibrary4.Models.DTOs.BorrowRecorddto;

public class BorrowRecordDto
{
    public int Id { get; set; } // Уникальный идентификатор записи

    public int BookId { get; set; } // ID книги
        
    public string? BookTitle { get; set; } // Название книги (может быть null)

    public int UserId { get; set; } // ID пользователя
        
    public string? UserName { get; set; } // Имя пользователя (может быть null)

    public DateTime BorrowDate { get; set; } // Дата взятия

    public DateTime? ReturnDate { get; set; } // Дата возврата (может быть null
}