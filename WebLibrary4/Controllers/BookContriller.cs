using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.Entities;
 
 

namespace WebLibrary4.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }
        
        [HttpGet("test-exception")]
        public IActionResult TestException()
        {
            throw new Exception("Тест выброса исключения");
        }
     

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string? title)
        {
            var books = await _bookService.SearchBooksAsync(title);

            if (!books.Any())
            {
                return NotFound("Книги не найдены.");
            }

            return Ok(books);
        }
        
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Номер страницы и размер страницы должны быть больше 0.");
            }

            // Получение общего количества книг
            var totalBooks = await _bookService.GetTotalBookCountAsync();

            // Расчет общего количества страниц
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            if (page > totalPages)
            {
                return BadRequest("Указанная страница выходит за пределы допустимого диапазона.");
            }

            // Получение книг для текущей страницы
            var books = await _bookService.GetBooksPaginatedAsync(page, pageSize);

            // Формирование результата
            var result = new
            {
                Books = _mapper.Map<IEnumerable<BookShortDto>>(books), // Преобразование сущностей в DTO
                CurrentPage = page,
                TotalPages = totalPages,
                TotalBooks = totalBooks
            };

            return Ok(result);
        }


        [HttpGet("get-all-simple")]
        public async Task<IActionResult> GetAllSimplified()
        {
            
             
                 
                var books = await _bookService.GetAllBooksAsync();

                
                var bookDtos = _mapper.Map<List<BookShortDto>>(books);

                return Ok(bookDtos);
            
             
        }

        [HttpGet("get-all-book")]
        public async Task<IActionResult> GetAll()
        {
            
                 
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            
             
        }

        [HttpGet("get-book/{id}")]
        public async Task<IActionResult> GetById(int id)
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

        [HttpPost("create-book")]
        public async Task<IActionResult> Create([FromBody] BookRequestDto bookDto)
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
                                                         || string.IsNullOrWhiteSpace(bookDto.Description) ||
                                                         string.IsNullOrWhiteSpace(bookDto.Genre)
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
            book.Id = await _bookService.AddBookAsync(book);

            // Маппинг результата Books -> BookResponseDto
            var bookResponse = _mapper.Map<BookResponseDto>(book);

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, bookResponse);

        }

        // [HttpPost("{bookId}/add-pdf")]
        // public async Task<IActionResult> UploadPdf(int bookId, [FromForm] IFormFile pdfFile)
        // {
        //
        //     // Вызываем сервис для обработки загрузки
        //     var pdfId = await _bookService.UploadPdfAsync(bookId, pdfFile);
        //
        //     // Проверяем результат и возвращаем успешный ответ
        //     return Ok(new
        //     {
        //         Message = "PDF успешно добавлен.",
        //         PdfId = pdfId
        //     });
        // }
        //

       
        
      

        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> UpdateDescription(int id, [FromBody] BookUpdateDiscrptionDto bookDto)
        {
            try
            {
                 
                if (bookDto == null || string.IsNullOrWhiteSpace(bookDto.Description))
                {
                    return BadRequest(new
                    {
                        Error = "Некорректные данные книги",
                        Details = "Описание книги не может быть пустым."
                    });
                }

                
                bool isUpdated = await _bookService.UpdateBookDescriptionAsync(id, bookDto);

                if (!isUpdated)
                {
                   
                    return NotFound(new
                    {
                        Error = "Книга не найдена",
                        Details = $"Книга с идентификатором {id} не существует."
                    });
                }

                
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                
                return BadRequest(new
                {
                    Error = "Ошибка валидации данных",
                    Details = ex.Message
                });
            }
            
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
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
    }
}