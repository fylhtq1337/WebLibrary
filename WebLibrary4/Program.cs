using Microsoft.AspNetCore.Diagnostics;
 using WebLibrary4.Interfaces;
using WebLibrary4.Repositories;
using WebLibrary4.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройка сервисов в методе ConfigureServices
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Настройка Middleware и конвейера запросов в методе Configure
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Регистрация контроллеров с централизованными настройками JSON
    services.AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            })
            .AddNewtonsoftJson(); // Добавление Newtonsoft.Json, если используется

    // Регистрация строки подключения как Singleton
    services.AddSingleton(provider => configuration.GetConnectionString("DefaultConnection"));

    // Регистрация зависимостей
    services.AddScoped<IBookRepository, BookRepository>(provider =>
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return new BookRepository(connectionString);
    });

    services.AddScoped<IBookService, BookService>();

    services.AddScoped<IClientRepository>(provider =>
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return new ClientRepository(connectionString);
    });

    services.AddScoped<IClientService, ClientService>();

    services.AddScoped<IBorrowRecordService, BorrowRecordService>();
    services.AddScoped<IBorrowRecordRepository, BorrowRecordRepository>();

    // Регистрация AutoMapper
    services.AddAutoMapper(typeof(Program));
}

void ConfigureMiddleware(WebApplication app)
{
    // Обработка исключений
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Кастомная обработка исключений
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500; // Внутренняя ошибка сервера
            context.Response.ContentType = "application/json";

            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = errorFeature?.Error;

            // Используем стандартный логгер для записи логов об ошибках
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Произошла необработанная ошибка");

            await context.Response.WriteAsync("Произошла ошибка.");
        });
    });

    app.UseHttpsRedirection();
    app.UseRouting();

     

    // Маршрутизация и инициализация статических файлов
    app.MapStaticAssets();

    app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();
}