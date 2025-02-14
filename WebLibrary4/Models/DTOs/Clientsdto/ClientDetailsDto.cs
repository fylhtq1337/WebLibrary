using WebLibrary4.Models.DTOs.BorrowRecorddto;

namespace WebLibrary4.Models.DTOs.Clientsdto;

public class ClientDetailsDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }  
    public ICollection<BorrowRecordDto> BorrowRecords { get; set; }
}