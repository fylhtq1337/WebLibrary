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
        
        public async Task<IEnumerable<Clients>> SearchByNameAsync(string name)
        {
            // Проверка на пустое или null имя
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя для поиска не может быть пустым.");
            }

            // Вызов метода репозитория
            return await _clientRepository.SearchByNameAsync(name);
        }

        public async Task<IEnumerable<Clients>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Clients?> GetClientByIdAsync(int id)
        {
            // Здесь можно добавить дополнительную логику, например, проверку ID
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

            // Преобразуем ClientCreateDto в модель Clients
            var client = new Clients
            {
                Username = clientDto.Username,
                Email = clientDto.Email,
                Role = "Client" // Указать значение по умолчанию, если Role отсутствует в DTO
            };

            // Передаём данные в репозиторий
            var id = await _clientRepository.AddAsync(client);

            // Если id больше 0, клиент был добавлен
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

            // Преобразуем ClientDetailsDto в модель Clients
            var client = new Clients
            {
                Id = clientDto.Id,
                Username = clientDto.Username,
                Email = clientDto.Email,
                Role = "Client" // Можно задать значение по умолчанию, если Role отсутствует в DTO
            };

            // Осуществляем обновление клиента
            await _clientRepository.UpdateAsync(client);

            return true; // Считаем успешным обновление, если нет ошибок
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id должен быть положительным числом.", nameof(id));
            }

            // Удаляем клиента через репозиторий
            return await _clientRepository.DeleteAsync(id);
        }
    }
}