namespace WebLibrary4.Models.DTOs;

public class BookShortDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public int Amount { get; set; }
}