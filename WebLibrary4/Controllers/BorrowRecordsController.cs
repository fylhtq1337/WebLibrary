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
                // Вызов метода сервиса для получения детализированных записей
                var detailedRecords = await _service.GetDetailedBorrowRecordsAsync();

                // Возвращаем результат
                return Ok(detailedRecords);
            }
            catch (Exception ex)
            {
                // Логирование исключений при необходимости
                Console.WriteLine($"Ошибка: {ex.Message}");

                // Возвращаем статус ошибки (500 — Internal server error)
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
            // Устанавливаем дату возврата для записи
            var updated = await _service.MarkAsReturnedAsync(id);

            if (!updated)
                return NotFound(); // Если запись с указанным Id не найдена

            return NoContent();
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