 
using Npgsql;
using WebLibrary4.Models.Entities;

namespace WebLibrary4.Repositories
{
    public class BorrowRecordRepository
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
                             UserId = reader.GetInt32(3),
                             BorrowDate = reader.GetDateTime(5),
                            ReturnDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
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
                             UserId = reader.GetInt32(3),
                             BorrowDate = reader.GetDateTime(5),
                            ReturnDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
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
                    @"INSERT INTO BorrowRecord (BookId, UserId, BorrowDate, ReturnDate) 
                      VALUES (@BookId, @BookTitle, @UserId, @UserName, @BorrowDate, @ReturnDate) 
                      RETURNING Id", 
                    connection
                );

                command.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                 command.Parameters.AddWithValue("@UserId", borrowRecord.UserId);
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
                          
                          UserId = @UserId, 
                           
                          BorrowDate = @BorrowDate, 
                          ReturnDate = @ReturnDate 
                      WHERE Id = @Id", 
                    connection
                );

                command.Parameters.AddWithValue("@Id", borrowRecord.Id);
                command.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                 command.Parameters.AddWithValue("@UserId", borrowRecord.UserId);
                 command.Parameters.AddWithValue("@BorrowDate", borrowRecord.BorrowDate);
                command.Parameters.AddWithValue("@ReturnDate", borrowRecord.ReturnDate ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
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