using WebLibrary4.Models.DTOs.Clientsdto;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces
{
    public interface IClientService
    {
         
        Task<IEnumerable<Clients>> GetPaginatedClientsAsync(int page, int pageSize);
        Task<int> GetTotalClientsCountAsync();
        Task<ClientSearchResult> SearchByNameAsync(string name, int page, int pageSize);
        Task<IEnumerable<Clients>> GetAllClientsAsync();    
        Task<Clients?> GetClientByIdAsync(int id);         
        Task<bool> CreateClientAsync(ClientCreateDto client);          
        Task<bool> UpdateClientAsync(ClientDetailsDto client);      
        Task<bool> DeleteClientAsync(int id);            
    }
}