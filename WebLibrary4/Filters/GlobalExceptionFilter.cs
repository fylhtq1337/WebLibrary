using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace WebLibrary4.Filters
{
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        // Логируем информацию об ошибке
        _logger.LogError(context.Exception, "Произошла ошибка");

        // Формируем ответ (JSON, текст и т.д.)
        // Ниже пример простого JSON-ответа
        context.Result = new JsonResult(new 
        {
            Message = "Что-то пошло не так, попробуйте позже",
            Error = context.Exception.Message // не всегда стоит выдавать пользователю текст исключения
        })
        {
            StatusCode = 500
        };

        // Показываем, что исключение обработано и больше обрабатываться не будет
        context.ExceptionHandled = true;
    }
}

}