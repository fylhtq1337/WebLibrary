using WebLibrary4.Models.Entities;

namespace WebLibrary4.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Clients>> SearchByNamePaginatedAsync(string name, int page, int pageSize);
        Task<int> CountByNameAsync(string name);
        Task<IEnumerable<Clients>> GetPaginatedClientsAsync(int skip, int take);
        Task<int> GetTotalClientsCountAsync();
        Task<IEnumerable<Clients>> GetAllAsync();       
        Task<Clients?> GetByIdAsync(int id);           
        Task<int> AddAsync(Clients client);                
        Task UpdateAsync(Clients client);              
        Task<bool> DeleteAsync(int id);                  
    }
}