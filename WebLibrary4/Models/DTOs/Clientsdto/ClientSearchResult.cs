using WebLibrary4.Models.Entities;

namespace WebLibrary4.Models.DTOs.Clientsdto;

public class ClientSearchResult
{
    public IEnumerable<Clients> Clients { get; set; }  
    public int CurrentPage { get; set; }  
    public int TotalPages { get; set; }  
    public int TotalClients { get; set; }  
}