using Microsoft.AspNetCore.Mvc;
using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs.Clientsdto;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedClients([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Номер страницы и размер страницы должны быть больше нуля.");
            }

            try
            {
                var clients = await _clientService.GetPaginatedClientsAsync(page, pageSize);
                if (!clients.Any())
                {
                    return NotFound("Клиентов не найдено.");
                }

                // Формируем результат с учетом общих страниц
                var totalClients = await _clientService.GetTotalClientsCountAsync();
                var totalPages = (int)Math.Ceiling((double)totalClients / pageSize);

                return Ok(new
                {
                    Clients = clients,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalClients = totalClients
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            try
            {
                // Вызов метода сервиса
                var clients = await _clientService.SearchByNameAsync(name);

                if (!clients.Any())
                {
                    return NotFound($"Клиенты с именем '{name}' не найдены.");
                }

                return Ok(clients);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound(new { Message = "Клиент не найден" });
            }

            return Ok(client);
        }

         
        [HttpPost("create")]
        public async Task<IActionResult> CreateClient([FromBody] ClientCreateDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(clientDto.Username) || string.IsNullOrWhiteSpace(clientDto.Email))
            {
                return BadRequest(new { Message = "Имя клиента и email обязательны" });
            }

            if (!IsValidEmail(clientDto.Email)) // Можно добавить кастомную валидацию email
            {
                return BadRequest(new { Message = "Некорректный email" });
            }

            var isCreated = await _clientService.CreateClientAsync(clientDto);

            if (!isCreated)
            {
                return BadRequest(new { Message = "Не удалось создать клиента." });
            }

            return Ok(new { Message = "Клиент успешно создан." });
        }

// Метод проверки валидности email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientDetailsDto clientDto)
        {
            if (id != clientDto.Id)
            {
                return BadRequest("Id клиента в URL не совпадает с Id в теле запроса.");
            }

            var result = await _clientService.UpdateClientAsync(clientDto);

            if (!result)
            {
                return BadRequest(new { Message = "Не удалось обновить клиента." });
            }

            return Ok(new { Message = "Клиент успешно обновлен." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var result = await _clientService.DeleteClientAsync(id);

            if (!result)
            {
                return NotFound(new { Message = "Клиент для удаления не найден" });
            }

            return Ok(new { Message = "Клиент успешно удалён" });
        }
    }
}