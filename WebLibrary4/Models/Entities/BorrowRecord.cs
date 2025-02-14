using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLibrary4.Models.Entities;

public class BorrowRecord
{
    public int Id { get; set; }  
    
    [Required(ErrorMessage = "Книга должна быть указана.")]
    [ForeignKey("Book")]  
    public int BookId { get; set; }  
    
    
    [Required(ErrorMessage = "Пользователь должен быть указан.")]
    [ForeignKey("Clints")]
    public int ClientId { get; set; }  
    
     
    
    [Required(ErrorMessage = "Дата взятия книги обязательна.")]
    [DataType(DataType.Date)]
    public DateTime BorrowDate { get; set; }  
    
    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; }  

     
    public Books? BorrowedBook { get; set; }  
    public Clients? Borrower { get; set; }  
    
    
     
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