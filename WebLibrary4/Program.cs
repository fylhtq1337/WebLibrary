using Microsoft.AspNetCore.Diagnostics;
using WebLibrary4.Interfaces;
using WebLibrary4.Repositories;
using WebLibrary4.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
 
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });


builder.Services.AddScoped<IBookRepository, BookRepository>((provider) =>
{
    // Реализация фабрики для построения BookRepository
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new BookRepository(connectionString);
});
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddAutoMapper(typeof(Program)); // Автоматически найдет BookMappingProfile
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500; // Внутренняя ошибка сервера
        context.Response.ContentType = "application/json";

        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = errorFeature?.Error;

        Console.WriteLine(exception); // Вывод сообщения об ошибке в консоль

        await context.Response.WriteAsync("Произошла ошибка.");
    });
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();