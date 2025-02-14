using System.ComponentModel.DataAnnotations;

namespace WebLibrary4.Models.DTOs;

public class BookRequestDto

{
    public int Id { get; set; }
    [Required(ErrorMessage = "Название книги обязательно для заполнения.")]
    [StringLength(100, ErrorMessage = "Название книги не должно превышать 100 символов.")]
    public string Title { get; set; } = string.Empty;  
    
    [StringLength(500, ErrorMessage = "Описание книги не должно превышать 500 символов.")]
    public string Description { get; set; } = string.Empty;  
    
    [Required(ErrorMessage = "Автор книги обязателен для заполнения.")]
    [StringLength(100, ErrorMessage = "Имя автора не должно превышать 100 символов.")]
    public string Author { get; set; }  
    
    [Required(ErrorMessage = "Жанр книги обязателен для заполнения.")]
    [StringLength(50, ErrorMessage = "Жанр не должен превышать 50 символов.")]
    public string Genre { get; set; }  
    
    [Range(1000, 2026, ErrorMessage = "Год выпуска должен быть в диапазоне от 1000 до 2026.")]
    public int Year { get; set; }  
    
    [Range(1, int.MaxValue, ErrorMessage = "Количество экземпляров книги должно быть больше 0.")]
    public int Amount { get; set; }  
}