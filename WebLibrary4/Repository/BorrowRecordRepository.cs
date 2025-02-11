 
using System.Diagnostics;
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
        
        public async Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> SearchBorrowRecordsAsync(string? bookTitle, string? clientName)
{
    var detailedRecords = new List<BorrowRecordClientNameBookTitleDto>();

    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        // Формируем базовый SQL-запрос
        var query = @"
            SELECT
                br.Id AS RecordId,
                c.Username AS ClientName,      -- Имя клиента
                b.Title AS BookTitle,          -- Название книги
                br.BorrowDate AS BorrowDate,   -- Дата взятия книги
                br.ReturnDate AS ReturnDate    -- Дата возврата книги (если есть)
            FROM
                BorrowRecord br
                JOIN Clients c ON br.ClientId = c.Id
                JOIN Books b ON br.BookId = b.Id
            WHERE
                (@BookTitle IS NULL OR b.Title ILIKE '%' || @BookTitle || '%') AND
                (@ClientName IS NULL OR c.Username ILIKE '%' || @ClientName || '%')";

        // Подготавливаем команду с параметрами
        var command = new NpgsqlCommand(query, connection);

        // Добавляем параметры (null-значение для игнорирования фильтра)
        command.Parameters.AddWithValue("@BookTitle", (object?)bookTitle ?? DBNull.Value);
        command.Parameters.AddWithValue("@ClientName", (object?)clientName ?? DBNull.Value);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                detailedRecords.Add(new BorrowRecordClientNameBookTitleDto
                {
                    Id = reader.GetInt32(0),                              // Id записи 
                    ClientName = reader.GetString(1),                     // Имя клиента
                    BookTitle = reader.GetString(2),                      // Название книги
                    BorrowDate = reader.GetDateTime(3),                   // Дата выдачи
                    ReturnDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4) // Дата возврата
                });
            }
        }
    }

    return detailedRecords;
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

        // Добавить новую запись и уменьшить количество доступных книг
public async Task<int> AddAsync(BorrowRecord borrowRecord)
{
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var transaction = await connection.BeginTransactionAsync())
        {
            try
            {
                // Проверить, доступна ли книга для выдачи
                var checkAmountQuery = "SELECT Amount FROM Books WHERE Id = @BookId";
                var commandCheckAmount = new NpgsqlCommand(checkAmountQuery, connection, transaction);
                commandCheckAmount.Parameters.AddWithValue("@BookId", borrowRecord.BookId);

                var amount = await commandCheckAmount.ExecuteScalarAsync();
                
                // Конвертируем результат в int и проверяем количество книг
                if (amount == null || Convert.ToInt32(amount) <= 0)
                {
                    throw new Exception("Книга недоступна для выдачи. Остаток экземпляров равен 0.");
                }
                
                // Уменьшить количество книг, если доступные экземпляры больше 0
                var updateBookQuery = @"
                    UPDATE Books
                    SET Amount = Amount - 1
                    WHERE Id = @BookId AND Amount > 0
                    RETURNING Amount";

                var commandUpdateBook = new NpgsqlCommand(updateBookQuery, connection, transaction);
                commandUpdateBook.Parameters.AddWithValue("@BookId", borrowRecord.BookId);

                // Получаем новое значение Amount
                var updatedAmount = await commandUpdateBook.ExecuteScalarAsync();
                if (updatedAmount == null)
                {
                    throw new Exception("Книга недоступна для выдачи (количество экземпляров = 0).");
                }

                // Добавление новой записи в BorrowRecord
                var insertQuery = @"
                    INSERT INTO BorrowRecord (BookId, ClientId, BorrowDate, ReturnDate) 
                    VALUES (@BookId, @ClientId, @BorrowDate, @ReturnDate) 
                    RETURNING Id";

                var commandInsert = new NpgsqlCommand(insertQuery, connection, transaction);
                commandInsert.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                commandInsert.Parameters.AddWithValue("@ClientId", borrowRecord.ClientId);
                commandInsert.Parameters.AddWithValue("@BorrowDate", borrowRecord.BorrowDate);
                commandInsert.Parameters.AddWithValue("@ReturnDate", borrowRecord.ReturnDate ?? (object)DBNull.Value);

                var result = await commandInsert.ExecuteScalarAsync();

                // Фиксация транзакции
                await transaction.CommitAsync();

                return Convert.ToInt32(result);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

       // Обновить запись и вернуть книгу
public async Task UpdateAsync(BorrowRecord borrowRecord)
{
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var transaction = await connection.BeginTransactionAsync())
        {
            try
            {
                // Проверить, была ли книга уже возвращена
                var checkQuery = @"
                    SELECT ReturnDate
                    FROM BorrowRecord
                    WHERE Id = @Id";

                var commandCheck = new NpgsqlCommand(checkQuery, connection, transaction);
                commandCheck.Parameters.AddWithValue("@Id", borrowRecord.Id);

                var existingReturnDate = await commandCheck.ExecuteScalarAsync();
                if (existingReturnDate != DBNull.Value && existingReturnDate != null)
                {
                    throw new Exception("Книга уже возвращена.");
                }

                // Увеличить количество доступных книг
                var updateBookQuery = @"
                    UPDATE Books
                    SET Amount = Amount + 1
                    WHERE Id = @BookId";

                var commandUpdateBook = new NpgsqlCommand(updateBookQuery, connection, transaction);
                commandUpdateBook.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                await commandUpdateBook.ExecuteNonQueryAsync();

                // Обновить запись BorrowRecord
                var updateBorrowRecordQuery = @"
                    UPDATE BorrowRecord
                    SET BookId = @BookId,
                        ClientId = @ClientId,
                        BorrowDate = @BorrowDate,
                        ReturnDate = @ReturnDate
                    WHERE Id = @Id";

                var commandUpdateBorrowRecord = new NpgsqlCommand(updateBorrowRecordQuery, connection, transaction);
                commandUpdateBorrowRecord.Parameters.AddWithValue("@Id", borrowRecord.Id);
                commandUpdateBorrowRecord.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                commandUpdateBorrowRecord.Parameters.AddWithValue("@ClientId", borrowRecord.ClientId);
                commandUpdateBorrowRecord.Parameters.AddWithValue("@BorrowDate", borrowRecord.BorrowDate);
                commandUpdateBorrowRecord.Parameters.AddWithValue("@ReturnDate", borrowRecord.ReturnDate ?? (object)DBNull.Value);

                await commandUpdateBorrowRecord.ExecuteNonQueryAsync();

                // Фиксация транзакции
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
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
                br.Id AS RecordId,
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
                        int recordId = reader.GetInt32(0);
                        Debug.WriteLine($"Record ID fetched from DB: {recordId}");
                        detailedRecords.Add(new BorrowRecordClientNameBookTitleDto
                        {
                            Id = reader.GetInt32(0),                              // RecordId (Id)
                            ClientName = reader.GetString(1),                     // ClientName
                            BookTitle = reader.GetString(2),                      // BookTitle
                            BorrowDate = reader.GetDateTime(3),                   // BorrowDate
                            ReturnDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4) // ReturnDate
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