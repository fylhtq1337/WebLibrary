using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Models.Entities;
 

namespace WebLibrary4.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        
        public async Task<int?> UploadPdfAsync(int bookId, IFormFile pdfFile)
        {
            // 1. Проверяем, что файл передан
            if (pdfFile == null)
                throw new ArgumentException("Файл не передан. Пожалуйста, загрузите PDF-файл.");

            // 2. Проверяем MIME-тип файла
            if (pdfFile.ContentType != "application/pdf")
                throw new ArgumentException("Некорректный файл. Допускаются только PDF-файлы.");

            // 3. Ограничиваем размер файла (например, 500 MB)
            const long maxFileSize = 500L * 1024 * 1024; // 500 MB
            if (pdfFile.Length > maxFileSize)
                throw new ArgumentException($"Размер файла превышает ограничение в {maxFileSize / (1024 * 1024)} MB.");

            // 4. Проверяем существование книги в базе данных
            var existingBook = await _bookRepository.GetByIdAsync(bookId);
            if (existingBook == null)
                throw new KeyNotFoundException($"Книга с идентификатором {bookId} не найдена.");

            // 5. Читаем содержимое файла
            using var memoryStream = new MemoryStream();
            await pdfFile.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // 6. Создаём объект PdfDocument для добавления в базу данных
            var pdfDocument = new PdfDocument
            {
                FileName = pdfFile.FileName,
                Content = fileBytes,
                ContentType = pdfFile.ContentType
            };

            // 7. Сохраняем PDF-файл через репозиторий
            var pdfId = await _bookRepository.AddPdfAsync(pdfDocument, bookId);

            // 8. Возвращаем ID добавленного PDF (или null, если процесс завершился неудачно)
            return pdfId;
        }

        public async Task<IEnumerable<Books>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Books?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<int> AddBookAsync(Books book)
        {
            return await _bookRepository.AddAsync(book);
        }
        
        public async Task<PdfDocument> ShowContetnBook(int id)
        {
            // Шаг 1: Получение файла PDF из репозитория
            var pdf = await _bookRepository.ReturnPdf(id);

            // Шаг 2: Проверка результата (если PDF не найден, выбрасываем исключение)
            if (pdf == null)
            {
                throw new KeyNotFoundException($"PDF-книга с Id = {id} не найдена.");
            }

            // Шаг 3: Возврат PDF на вызвавший метод
            return pdf;
        }

        public async Task<bool> UpdateBookDescriptionAsync(int id, BookUpdateDiscrptionDto dto)
        {
            // Проверяем, что DTO не null
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Описание книги не может быть пустым.");
            }

            // Проверяем, существует ли книга с переданным Id
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                // Если книга не найдена, возвращаем false
                return false;
            }

            // Обновляем только поле Description
            existingBook.Description = dto.Description;

            // Сохраняем изменения через репозиторий
            await _bookRepository.UpdateAsync(existingBook);

            // Возвращаем true, если обновление прошло успешно
            return true;
        }
        
        public async Task<int?> AddPdfToBookAsync(PdfDocument pdf, int bookId)
        {
            // Проверяем существование книги
            var existingBook = await _bookRepository.GetByIdAsync(bookId);
            if (existingBook == null)
            {
                // Если книги не существует, возвращаем null (или бросаем исключение, если нужно)
                return null;
            }

            // Добавляем PDF через репозиторий
            var pdfId = await _bookRepository.AddPdfAsync(pdf, bookId);
            return pdfId; // Возвращаем ID добавленного PDF
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            // Проверяем, существует ли книга
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                // Если книги нет, возвращаем false
                return false;
            }

            // Если книга существует, удаляем её
            await _bookRepository.DeleteAsync(existingBook.Id);
            return true; // Успешно удалено
        }
    }
}