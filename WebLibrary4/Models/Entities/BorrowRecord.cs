using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLibrary4.Models.Entities;

public class BorrowRecord
{
    public int Id { get; set; } // Уникальный идентификатор записи
    
    [Required(ErrorMessage = "Книга должна быть указана.")]
    [ForeignKey("Book")] // Указываем, что BookId ссылается на Book
    public int BookId { get; set; } // ID книги
    
    [StringLength(100, ErrorMessage = "Название книги не должно превышать 100 символов.")]
    public string BookTitle { get; set; } // Название книги (если имеется)
    
    [Required(ErrorMessage = "Пользователь должен быть указан.")]
    [ForeignKey("User")]
    public int UserId { get; set; } // ID пользователя
    
    [StringLength(100, ErrorMessage = "Имя пользователя не должно превышать 100 символов.")]
    public string UserName { get; set; } // Имя пользователя (если имеется)
    
    [Required(ErrorMessage = "Дата взятия книги обязательна.")]
    [DataType(DataType.Date)]
    public DateTime BorrowDate { get; set; } // Дата взятия
    
    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; } // Дата возврата (может быть null)

    // Связи (для удобства)
    public Books? BorrowedBook { get; set; } // Ссылка на книгу
    public Users? Borrower { get; set; } // Ссылка на пользователя
    
    
    // Метод для проверки корректности даты возврата
    public static ValidationResult? ValidateReturnDate(DateTime? returnDate, ValidationContext context)
    {
        var instance = (BorrowRecord)context.ObjectInstance;

        if (returnDate.HasValue && returnDate.Value < instance.BorrowDate)
        {
            return new ValidationResult("Дата возврата не может быть раньше даты взятия.");
        }

        return ValidationResult.Success;
    }
}