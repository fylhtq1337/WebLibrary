namespace WebLibrary4.Models.DTOs.BorrowRecorddto;

public class BorrowRecordDto
{
    public int Id { get; set; } // Уникальный идентификатор записи

    public int BookId { get; set; } // ID книги
        
 
    public int ClientId { get; set; } // ID пользователя
        
 
    public DateTime BorrowDate { get; set; } // Дата взятия

    public DateTime? ReturnDate { get; set; } // Дата возврата (может быть null
}