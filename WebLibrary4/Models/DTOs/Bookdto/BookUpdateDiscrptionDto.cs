namespace WebLibrary4.Models.DTOs;
using System.ComponentModel.DataAnnotations;

public class BookUpdateDiscrptionDto
{
     
    [StringLength(500, ErrorMessage = "Описание книги не должно превышать 500 символов.")]
    public string Description { get; set; } = string.Empty;  

}