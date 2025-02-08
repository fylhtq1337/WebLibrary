using System.ComponentModel.DataAnnotations;

namespace WebLibrary4.Models.Entities;

public class Users
{
    public int Id { get; set; } // Уникальный идентификатор
    
    [Required(ErrorMessage = "Имя  Клиента обязательно для заполнения.")]
    [StringLength(100, ErrorMessage = "Имя  Клиента не должно превышать 100 символов.")]
    public string Username { get; set; } = string.Empty; // Логин пользователя
     
    [Required(ErrorMessage = "Email обязателен для заполнения.")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email адреса.")]
    [StringLength(255, ErrorMessage = "Email не может превышать 255 символов.")]
    public string Email {get; set;}
    
    public string Role { get; set; } = "Client"; // Роль (по умолчанию "Client")
    
    public ICollection<BorrowRecord> BorrowRecords { get; set; }
}