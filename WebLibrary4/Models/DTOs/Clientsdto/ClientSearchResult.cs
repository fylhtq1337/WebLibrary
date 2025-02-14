using WebLibrary4.Models.Entities;

namespace WebLibrary4.Models.DTOs.Clientsdto;

public class ClientSearchResult
{
    public IEnumerable<Clients> Clients { get; set; } // Клиенты текущей страницы
    public int CurrentPage { get; set; } // Текущая страница
    public int TotalPages { get; set; } // Общее количество страниц
    public int TotalClients { get; set; } // Общее количество найденных клиентов
}