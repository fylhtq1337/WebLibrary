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

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BorrowRecordDto dto)
        {
            var newId = await _service.CreateBorrowAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, dto);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] BorrowRecordDto dto)
        {
            await _service.UpdateAsync(dto);
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