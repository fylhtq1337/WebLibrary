using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebLibrary4.Models.Entities;
using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs.Clientsdto;

namespace WebLibrary4.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        
        public async Task<IEnumerable<Clients>> GetPaginatedClientsAsync(int page, int pageSize)
        {
             
            var skip = (page - 1) * pageSize;

             
            return await _clientRepository.GetPaginatedClientsAsync(skip, pageSize);
        }

        public async Task<int> GetTotalClientsCountAsync()
        {
            return await _clientRepository.GetTotalClientsCountAsync();
        }
        
        public async Task<ClientSearchResult> SearchByNameAsync(string name, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя для поиска не может быть пустым.");
            }

             
            var paginatedClients = await _clientRepository.SearchByNamePaginatedAsync(name, page, pageSize);
            var totalClients = await _clientRepository.CountByNameAsync(name);  

            var totalPages = (int)Math.Ceiling((double)totalClients / pageSize);

            return new ClientSearchResult
            {
                Clients = paginatedClients,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalClients = totalClients
            };
        }

        public async Task<IEnumerable<Clients>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Clients?> GetClientByIdAsync(int id)
        {
             
            if (id <= 0)
            {
                throw new ArgumentException("Id должен быть положительным числом.", nameof(id));
            }
            
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateClientAsync(ClientCreateDto clientDto)
        {
            if (string.IsNullOrWhiteSpace(clientDto.Username))
            {
                throw new ArgumentException("Имя клиента не может быть пустым.", nameof(clientDto.Username));
            }

            if (string.IsNullOrWhiteSpace(clientDto.Email))
            {
                throw new ArgumentException("Email не может быть пустым.", nameof(clientDto.Email));
            }

            
            var client = new Clients
            {
                Username = clientDto.Username,
                Email = clientDto.Email,
                Role = "Client"  
            };

            
            var id = await _clientRepository.AddAsync(client);

            
            return id > 0;
        }

       

        public async Task<bool> UpdateClientAsync(ClientDetailsDto clientDto)
        {
            if (clientDto.Id <= 0)
            {
                throw new ArgumentException("Id клиента должен быть положительным числом.", nameof(clientDto.Id));
            }

            if (string.IsNullOrWhiteSpace(clientDto.Username))
            {
                throw new ArgumentException("Имя клиента не может быть пустым.", nameof(clientDto.Username));
            }

            if (string.IsNullOrWhiteSpace(clientDto.Email))
            {
                throw new ArgumentException("Email клиента не может быть пустым.", nameof(clientDto.Email));
            }

             
            var client = new Clients
            {
                Id = clientDto.Id,
                Username = clientDto.Username,
                Email = clientDto.Email,
                Role = "Client" 
            };

             
            await _clientRepository.UpdateAsync(client);

            return true;  
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id должен быть положительным числом.", nameof(id));
            }

            
            return await _clientRepository.DeleteAsync(id);
        }
    }
}