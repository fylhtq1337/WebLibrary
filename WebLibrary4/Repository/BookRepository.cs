using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using WebLibrary4.Interfaces; // PostgreSQL
using WebLibrary4.Models.Entities;
 
namespace WebLibrary4.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Books>> GetAllAsync()
        {
            var books = new List<Books>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM Books", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        books.Add(new Books
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Author = reader.GetString(3),
                            Genre = reader.GetString(4),
                            Year = reader.GetInt32(5),
                            Amount = reader.GetInt32(6)
                        });
                    }
                }
            }

            return books;
        }

        public async Task<Books?> GetByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM Books WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Books
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Author = reader.GetString(3),
                            Genre = reader.GetString(4),
                            Year = reader.GetInt32(5),
                            Amount = reader.GetInt32(6)
                        };
                    }
                }
            }

            return null;
        }

        public async Task<int> AddAsync(Books book)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    @"INSERT INTO Books (Title, Description, Author, Genre, Year, Amount) 
                      VALUES (@Title, @Description, @Author, @Genre, @Year, @Amount) 
                      RETURNING Id", 
                    connection
                );

                command.Parameters.AddWithValue("@Title", book.Title);
                command.Parameters.AddWithValue("@Description", book.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@Genre", book.Genre);
                command.Parameters.AddWithValue("@Year", book.Year);
                command.Parameters.AddWithValue("@Amount", book.Amount);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
        
        public async Task<int> AddPdfAsync(PdfDocument pdf, int bookId)
        {
            if (pdf == null) throw new ArgumentNullException(nameof(pdf));

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    @"INSERT INTO PdfDocument (FileName, Content, ContentType, BookId) 
                      VALUES (@FileName, @Content, @ContentType, @BookId) 
                      RETURNING Id",
                    connection
                );

                command.Parameters.AddWithValue("@FileName", pdf.FileName);
                command.Parameters.AddWithValue("@Content", pdf.Content);
                command.Parameters.AddWithValue("@ContentType", pdf.ContentType);
                command.Parameters.AddWithValue("@BookId", bookId);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result); // Возвращаем ID добавленного PDF
            }
        }

        public async Task UpdateAsync(Books book)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    @"UPDATE Books 
                      SET Title = @Title, Description = @Description, Author = @Author, 
                          Genre = @Genre, Year = @Year, Amount = @Amount 
                      WHERE Id = @Id", 
                    connection
                );

                command.Parameters.AddWithValue("@Id", book.Id);
                command.Parameters.AddWithValue("@Title", book.Title);
                command.Parameters.AddWithValue("@Description", book.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@Genre", book.Genre);
                command.Parameters.AddWithValue("@Year", book.Year);
                command.Parameters.AddWithValue("@Amount", book.Amount);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("DELETE FROM Books WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}