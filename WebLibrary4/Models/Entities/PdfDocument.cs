using System.ComponentModel.DataAnnotations;

namespace WebLibrary4.Models.Entities;

public class PdfDocument
{
    public int Id { get; set; }  

    [Required(ErrorMessage = "Файл PDF обязательно должен содержать данные.")]
    [CustomValidation(typeof(PdfDocument), nameof(ValidateContent))]
    public byte[] Content { get; set; } = Array.Empty<byte>();  
    
    [Required(ErrorMessage = "Имя файла обязательно.")]
    [StringLength(255, ErrorMessage = "Имя файла не должно превышать 255 символов.")]
    [RegularExpression(@"^[^<>:{}]*$", ErrorMessage = "Имя файла содержит запрещенные символы.")]
    public string FileName { get; set; } = string.Empty;  
    
    [Required(ErrorMessage = "Тип контента обязателен.")]
    [RegularExpression(@"application/pdf", ErrorMessage = "Неправильный тип файла. Ожидается PDF.")]
    public string ContentType { get; set; } = "application/pdf";  
    
    public int BookId { get; set; }
    
    
    public static ValidationResult? ValidateContent(byte[] content, ValidationContext context)
    {
        const int MaxFileSize = 500 * 1024 * 1024; // 500 МБ
        if (content == null || content.Length == 0)
        {
            return new ValidationResult("Файл не должен быть пустым.");
        }

        if (content.Length > MaxFileSize)
        {
            return new ValidationResult($"Размер файла не должен превышать {MaxFileSize / 1024 / 1024} МБ.");
        }

        return ValidationResult.Success;
    }
}