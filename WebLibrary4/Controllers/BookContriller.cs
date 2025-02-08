using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.Entities;
 
 

namespace WebLibrary4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet("get-all-book")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Ожидаемое выполнение метода:
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (ArgumentNullException ex) // Например, если сервис возвращает Null
            {
                return BadRequest(new
                {
                    Error = "Некорректные параметры запроса",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Ловим остальные ошибки
                return StatusCode(500, new
                {
                    Error = "Произошла внутренняя ошибка сервера",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("get-book/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) // Проверяем некорректный id
                {
                    return BadRequest(new
                    {
                        Error = "Некорректный идентификатор книги",
                        Details = "Идентификатор должен быть больше нуля."
                    });
                }

                var book = await _bookService.GetBookByIdAsync(id);

                if (book == null)
                {
                    return NotFound(new
                    {
                        Error = "Книга не найдена",
                        Details = $"Книга с идентификатором {id} не существует в базе данных."
                    });
                }

                return Ok(book);
            }
            catch (ArgumentException ex) // Конкретное исключение (если, например, логика выбрасывает `ArgumentException`)
            {
                return BadRequest(new
                {
                    Error = "Ошибка запроса",
                    Details = ex.Message
                });
            }
            catch (Exception ex) // Все остальные необработанные ошибки
            {
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Details = ex.Message // В рабочем окружении можно убрать это поле
                });
            }
        }

        [HttpPost("create-book")]
        public async Task<IActionResult> Create([FromBody] BookRequestDto bookDto)
        {
            try
            {
                
                // Логирование тела запроса
                Console.WriteLine("Пришёл запрос:");
                Console.WriteLine($"Title: {bookDto.Title}, Author: {bookDto.Author}");
                // Проверяем входные данные
                if (bookDto == null)
                {
                    return BadRequest(new
                    {
                        Error = "Книга не была передана",
                        Details = "Запрос не содержит данных для создания книги."
                    });
                }

                if (string.IsNullOrWhiteSpace(bookDto.Title) || string.IsNullOrWhiteSpace(bookDto.Author) 
                    || string.IsNullOrWhiteSpace(bookDto.Description) ||string.IsNullOrWhiteSpace(bookDto.Genre)
                     )
                {
                    return BadRequest(new
                    {
                        Error = "Некорректные данные",
                        Details = "Название и имя автора не могут быть пустыми."
                    });
                }

                // Маппинг BookRequestDto -> Books
                var book = _mapper.Map<Books>(bookDto);
                await _bookService.AddBookAsync(book);

                // Маппинг результата Books -> BookResponseDto
                var bookResponse = _mapper.Map<BookResponseDto>(book);
                return CreatedAtAction(nameof(GetById), new { id = book.Id }, bookResponse);
            }
            catch (ArgumentException ex) // Если AddBookAsync выбрасывает ArgumentException
            {
                return BadRequest(new
                {
                    Error = "Ошибка валидации данных",
                    Details = ex.Message
                });
            }
            catch (Exception ex) // Все остальные необработанные ошибки
            {
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Details = ex.Message // Уберите в production для скрытия технических деталей
                });
            }
        }

        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookRequestDto bookDto)
        {
            try
            {
                // Маппинг BookRequestDto -> Books
                var book = _mapper.Map<Books>(bookDto);
                // Проверяем, что идентификаторы совпадают
                if (id != book.Id)
                {
                    return BadRequest(new
                    {
                        Error = "Несоответствие идентификаторов",
                        Details = "Идентификатор книги в пути не совпадает с идентификатором в теле запроса."
                    });
                }

                // Проверяем входные данные
                if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Description))
                {
                    return BadRequest(new
                    {
                        Error = "Некорректные данные книги",
                        Details = "Название и Описание книги не могут быть пустыми."
                    });
                }

                // Пытаемся обновить книгу
                bool isUpdated = await _bookService.UpdateBookAsync(book);

                // Если метод сервиса вернул false, значит книга не найдена
                if (!isUpdated)
                {
                    return NotFound(new
                    {
                        Error = "Книга не найдена",
                        Details = $"Книга с идентификатором {id} не существует."
                    });
                }

                // Возвращаем 204 No Content, если обновление прошло успешно
                return NoContent();
            }
            catch (ArgumentException ex) // Например, исключения валидации на уровне бизнес-логики
            {
                return BadRequest(new
                {
                    Error = "Ошибка валидации данных",
                    Details = ex.Message
                });
            }
            catch (Exception ex) // Необработанные ошибки
            {
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Details = ex.Message // Уберите поле `Details` в production
                });
            }
        }

        [HttpDelete("delete-book/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Пытаемся удалить книгу
                var isDeleted = await _bookService.DeleteBookAsync(id);

                // Если книга для удаления не найдена
                if (!isDeleted)
                {
                    return NotFound(new
                    {
                        Error = "Книга не найдена",
                        Details = $"Книга с идентификатором {id} не существует."
                    });
                }

                // Если книга успешно удалена
                return NoContent(); // Возвращаем 204
            }
            catch (ArgumentException ex) // Например, ошибки в бизнес-логике
            {
                return BadRequest(new
                {
                    Error = "Некорректный запрос",
                    Details = ex.Message
                });
            }
            catch (Exception ex) // Необработанные ошибки
            {
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Details = ex.Message // Уберите `Details` в production
                });
            }
        }
    }
}