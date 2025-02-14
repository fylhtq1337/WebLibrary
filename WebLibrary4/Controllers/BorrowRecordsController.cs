using Microsoft.AspNetCore.Mvc;
using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs.BorrowRecorddto;

namespace WebLibrary4.Controllers
{
    [Route("api/borrow-records")]
    [ApiController]
    public class BorrowRecordsController : ControllerBase
    {
        private readonly IBorrowRecordService _service;

        public BorrowRecordsController(IBorrowRecordService service)
        {
            _service = service;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var records = await _service.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _service.GetByIdAsync(id);

            if (record == null)
                return NotFound();

            return Ok(record);
        }
        [HttpGet("get-detailed")]
        public async Task<IActionResult> GetDetailedBorrowRecords()
        {
            try
            {
                 
                var detailedRecords = await _service.GetDetailedBorrowRecordsAsync();

                 
                return Ok(detailedRecords);
            }
            catch (Exception ex)
            {
                 
                Console.WriteLine($"Ошибка: {ex.Message}");

                 
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BorrowRecordDto dto)
        {
            Console.WriteLine($"DTO: ClientId = {dto.ClientId}, BookId = {dto.BookId}, BorrowDate = {dto.BorrowDate}");

            try
            {
                var newId = await _service.CreateBorrowAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newId }, dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return BadRequest("Произошла ошибка при создании записи выдачи.");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] BorrowRecordDto dto)
        {
            await _service.UpdateAsync(dto);
            return NoContent();
        }
        
        [HttpPut("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
             
            var updated = await _service.MarkAsReturnedAsync(id);

            if (!updated)
                return NotFound();  

            return NoContent();
        }
        [HttpGet("search-by-book")]
        public async Task<IActionResult> SearchByBookTitle([FromQuery] string bookTitle)
        {
            if (string.IsNullOrWhiteSpace(bookTitle))
                return BadRequest("Название книги не должно быть пустым.");

            try
            {
                var results = await _service.SearchByBookTitle(bookTitle);

                if (!results.Any())
                    return NotFound("Нет записей, связанных с указанной книгой.");

                return Ok(results);
            }
            catch (Exception ex)
            {
                 
                Console.WriteLine($"Ошибка поиска книги: {ex.Message}");

                 
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Message = ex.Message
                });
            }
        }
        
        [HttpGet("search-by-client")]
        public async Task<IActionResult> SearchByClientName([FromQuery] string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                return BadRequest("Имя клиента не должно быть пустым.");

            try
            {
                var results = await _service.SearchByClientName(clientName);

                if (!results.Any())
                    return NotFound("Нет записей, связанных с указанным клиентом.");

                return Ok(results);
            }
            catch (Exception ex)
            {
                 
                Console.WriteLine($"Ошибка поиска клиента: {ex.Message}");

                 
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Message = ex.Message
                });
            }
        }   
        

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var wasDeleted = await _service.DeleteAsync(id);

            if (!wasDeleted)
                return NotFound();

            return NoContent();
        }
    }
}