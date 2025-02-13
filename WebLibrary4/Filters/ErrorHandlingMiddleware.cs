using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Продолжаем цепочку вызовов
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при обработке запроса");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Настраиваем статус и заголовки
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Формируем ответ в нужном формате. Можно расширять по желанию
        var result = new
        {
            Error = "Внутренняя ошибка сервера",
            Details = ex.Message // В продакшене обычно убирают или логируют в файл
        };

        // Возвращаем JSON
        return context.Response.WriteAsJsonAsync(result);
    }
}