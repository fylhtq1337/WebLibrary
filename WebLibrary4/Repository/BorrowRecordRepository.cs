 
using Npgsql;
using WebLibrary4.Interfaces;
using WebLibrary4.Models.DTOs.BorrowRecorddto;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Repositories
{
    public class BorrowRecordRepository : IBorrowRecordRepository
    {
        private readonly string _connectionString;

        public BorrowRecordRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Получить все записи
        public async Task<IEnumerable<BorrowRecord>> GetAllAsync()
        {
            var borrowRecords = new List<BorrowRecord>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM BorrowRecord", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        borrowRecords.Add(new BorrowRecord
                        {
                            Id = reader.GetInt32(0),
                            BookId = reader.GetInt32(1),
                             ClientId = reader.GetInt32(2),
                             BorrowDate = reader.GetDateTime(3),
                            ReturnDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                        });
                    }
                }
            }

            return borrowRecords;
        }

        // Получить запись по Id
        public async Task<BorrowRecord?> GetByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM BorrowRecord WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new BorrowRecord
                        {
                            Id = reader.GetInt32(0),
                            BookId = reader.GetInt32(1),
                             ClientId = reader.GetInt32(2),
                             BorrowDate = reader.GetDateTime(3),
                            ReturnDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                        };
                    }
                }
            }

            return null;
        }

        // Добавить новую запись
        public async Task<int> AddAsync(BorrowRecord borrowRecord)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    @"INSERT INTO BorrowRecord (BookId, ClientId, BorrowDate, ReturnDate) 
                      VALUES (@BookId, @ClientId,  @BorrowDate, @ReturnDate) 
                      RETURNING Id", 
                    connection
                );

                command.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                 command.Parameters.AddWithValue("@ClientId", borrowRecord.ClientId);
                 command.Parameters.AddWithValue("@BorrowDate", borrowRecord.BorrowDate);
                command.Parameters.AddWithValue("@ReturnDate", borrowRecord.ReturnDate ?? (object)DBNull.Value);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        // Обновить запись
        public async Task UpdateAsync(BorrowRecord borrowRecord)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    @"UPDATE BorrowRecord
                      SET BookId = @BookId, 
                          
                          ClientId = @ClientId, 
                           
                          BorrowDate = @BorrowDate, 
                          ReturnDate = @ReturnDate 
                      WHERE Id = @Id", 
                    connection
                );

                command.Parameters.AddWithValue("@Id", borrowRecord.Id);
                command.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                 command.Parameters.AddWithValue("@ClientId", borrowRecord.ClientId);
                 command.Parameters.AddWithValue("@BorrowDate", borrowRecord.BorrowDate);
                command.Parameters.AddWithValue("@ReturnDate", borrowRecord.ReturnDate ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }
        
        public async Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> GetDetailedBorrowRecordsAsync()
        {
            var detailedRecords = new List<BorrowRecordClientNameBookTitleDto>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(@"
            SELECT
                c.Username AS ClientName,      -- Имя клиента
                b.Title AS BookTitle,          -- Название книги
                br.BorrowDate AS BorrowDate,   -- Дата взятия книги
                br.ReturnDate AS ReturnDate    -- Дата возврата книги (если есть)
            FROM
                BorrowRecord br
                JOIN Clients c ON br.ClientId = c.Id
                JOIN Books b ON br.BookId = b.Id", 
                    connection
                );

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detailedRecords.Add(new BorrowRecordClientNameBookTitleDto
                        {
                            ClientName = reader.GetString(0),                        // ClientName
                            BookTitle = reader.GetString(1),                        // BookTitle
                            BorrowDate = reader.GetDateTime(2),                     // BorrowDate
                            ReturnDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3) // ReturnDate
                        });
                    }
                }
            }

            return detailedRecords;
        }

        // Удалить запись по Id
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("DELETE FROM BorrowRecord WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }
    }
}