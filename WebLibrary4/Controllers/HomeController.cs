using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebLibrary4.Interfaces;
using WebLibrary4.Models;
using WebLibrary4.Models.DTOs;
using WebLibrary4.Services;

namespace WebLibrary4.Controllers;
// [Route("api/home")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookService _bookService;
    public HomeController(ILogger<HomeController> logger , IBookService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("books-list")]
    public async Task<IActionResult> BooksListPage()
    {
        try
        {
            // Получаем данные книг из сервиса
            var books = await _bookService.GetAllBooksAsync();

            // Преобразуем их в DTO
            var bookDtos = books.Select(book => new BookShortDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Description = book.Description,
                Year = book.Year,
                Amount = book.Amount
            }).ToList();

            // Передаём данные в представление
            return View("BookList", bookDtos);
        }
        catch (Exception ex)
        {
            // Возвращаем страницу ошибки или заглушку
            return StatusCode(500, new { Error = "Ошибка сервера", Details = ex.Message });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}