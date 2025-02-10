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
        
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetBookPdf(int id)
        {
            try
            {
                // Вызов метода сервиса
                var pdf = await _bookService.ShowContetnBook(id);

                // Возвращаем PDF-файл клиенту
                return File(pdf.Content, pdf.ContentType, pdf.FileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }
        
        
        
        
        [HttpGet("get-all-simple")]
        public async Task<IActionResult> GetAllSimplified()
        {
            Console.WriteLine("Метод вызван!");
            try
            {
                // Получаем полную информацию о книгах из `IBookService`
                var books = await _bookService.GetAllBooksAsync();

                // Маппинг через AutoMapper
                var bookDtos = _mapper.Map<List<BookShortDto>>(books);

                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                // Обработка исключений
                return StatusCode(500, new
                {
                    Error = "Ошибка сервера",
                    Details = ex.Message
                });
            }
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
       
        
        [HttpPost("{bookId}/add-pdf")]
        public async Task<IActionResult> UploadPdf(int bookId, [FromForm] IFormFile pdfFile)
        {
            try
            {
        // Проверяем, что файл передан
        if (pdfFile == null)
        {
            return BadRequest(new
            {
                Error = "Файл не передан",
                Details = "Пожалуйста, загрузите PDF-файл."
            });
        }

        // Проверяем MIME-тип файла
        if (pdfFile.ContentType != "application/pdf")
        {
            return BadRequest(new
            {
                Error = "Некорректный файл",
                Details = "Допускаются только PDF-файлы."
            });
        }

        // Ограничиваем размер файла (например, 500 МБ)
        const long maxFileSize = 500L * 1024 * 1024; // 500 MB
        if (pdfFile.Length > maxFileSize)
        {
            return BadRequest(new
            {
                Error = "Превышен допустимый размер файла",
                Details = $"Размер файла не должен превышать {maxFileSize / (1024 * 1024)} MB."
            });
        }

        // Читаем содержимое файла в массив байтов
        using var memoryStream = new MemoryStream();
        await pdfFile.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        // Создаём объект PdfDocument
        var pdfDocument = new PdfDocument
        {
            FileName = pdfFile.FileName,
            Content = fileBytes,
            ContentType = pdfFile.ContentType
        };

        // Сохраняем PDF-файл через `BookService`
        var pdfId = await _bookService.AddPdfToBookAsync(pdfDocument, bookId);

        if (pdfId == null)
        {
            return NotFound(new
            {
                Error = "Книга не найдена",
                Details = $"Книга с идентификатором {bookId} не существует в базе данных."
            });
        }

        // Возвращаем успешный результат
        return Ok(new
        {
            Message = "PDF успешно добавлен",
            PdfId = pdfId
        });
    }
    catch (Exception ex)
    {
        // Обработка ошибок
        return StatusCode(500, new
        {
            Error = "Внутренняя ошибка сервера",
            Details = ex.Message
        });
    }
}

        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> UpdateDescription(int id, [FromBody] BookUpdateDiscrptionDto bookDto)
        {
            try
            {
                // Проверяем входные данные DTO
                if (bookDto == null || string.IsNullOrWhiteSpace(bookDto.Description))
                {
                    return BadRequest(new
                    {
                        Error = "Некорректные данные книги",
                        Details = "Описание книги не может быть пустым."
                    });
                }

                // Пытаемся обновить описание книги через сервис
                bool isUpdated = await _bookService.UpdateBookDescriptionAsync(id, bookDto);

                if (!isUpdated)
                {
                    // Если книга не найдена
                    return NotFound(new
                    {
                        Error = "Книга не найдена",
                        Details = $"Книга с идентификатором {id} не существует."
                    });
                }

                // Возвращаем код 204 (No Content) при успешном обновлении
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                // Обрабатываем ошибки валидации
                return BadRequest(new
                {
                    Error = "Ошибка валидации данных",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Обрабатываем необработанные ошибки
                return StatusCode(500, new
                {
                    Error = "Внутренняя ошибка сервера",
                    Details = ex.Message // Уберите это поле в production
                });
            }
        }

        [HttpDelete("delete/{id}")]
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