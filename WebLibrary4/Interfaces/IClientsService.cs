using WebLibrary4.Models.DTOs.Clientsdto;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<Clients>> SearchByNameAsync(string name);
        Task<IEnumerable<Clients>> GetAllClientsAsync();   // Получить всех клиентов
        Task<Clients?> GetClientByIdAsync(int id);         // Получить клиента по ID
        Task<bool> CreateClientAsync(ClientCreateDto client);         // Добавить нового клиента
        Task<bool> UpdateClientAsync(ClientDetailsDto client);      // Обновить клиента
        Task<bool> DeleteClientAsync(int id);              // Удалить клиента
    }
}