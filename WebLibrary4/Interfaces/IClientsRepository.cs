using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Clients>> SearchByNameAsync(string name);
        Task<IEnumerable<Clients>> GetPaginatedClientsAsync(int skip, int take);
        Task<int> GetTotalClientsCountAsync();
        Task<IEnumerable<Clients>> GetAllAsync();       // Получить всех клиентов
        Task<Clients?> GetByIdAsync(int id);            // Получить клиента по Id
        Task<int> AddAsync(Clients client);                  // Создать нового клиента
        Task UpdateAsync(Clients client);               // Обновить клиента
        Task<bool> DeleteAsync(int id);                 // Удалить клиента по Id
    }
}