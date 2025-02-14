 
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
        
        public async Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> SearchByBookTitleAsync(string bookTitle)
        {
            var detailedRecords = new List<BorrowRecordClientNameBookTitleDto>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                 
                var query = @"
            SELECT
                br.Id AS RecordId,
                c.Username AS ClientName,       
                b.Title AS BookTitle,           
                br.BorrowDate AS BorrowDate,    
                br.ReturnDate AS ReturnDate     
            FROM
                BorrowRecord br
                JOIN Clients c ON br.ClientId = c.Id
                JOIN Books b ON br.BookId = b.Id
            WHERE
                b.Title ILIKE '%' || @BookTitle || '%'";
        
                
                var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookTitle", bookTitle);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detailedRecords.Add(new BorrowRecordClientNameBookTitleDto
                        {
                            Id = reader.GetInt32(0),                               
                            ClientName = reader.GetString(1),                     
                            BookTitle = reader.GetString(2),                      
                            BorrowDate = reader.GetDateTime(3),                   
                            ReturnDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4)  
                        });
                    }
                }
            }

            return detailedRecords;
        }
        
        public async Task<IEnumerable<BorrowRecordClientNameBookTitleDto>> SearchByClientNameAsync(string clientName)
        {
            var detailedRecords = new List<BorrowRecordClientNameBookTitleDto>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                
                var query = @"
            SELECT
                br.Id AS RecordId,
                c.Username AS ClientName,      
                b.Title AS BookTitle,           
                br.BorrowDate AS BorrowDate,    
                br.ReturnDate AS ReturnDate     
            FROM
                BorrowRecord br
                JOIN Clients c ON br.ClientId = c.Id
                JOIN Books b ON br.BookId = b.Id
            WHERE
                c.Username ILIKE '%' || @ClientName || '%'";
                
                 
                var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientName", clientName);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detailedRecords.Add(new BorrowRecordClientNameBookTitleDto
                        {
                            Id = reader.GetInt32(0),                              
                            ClientName = reader.GetString(1),                      
                            BookTitle = reader.GetString(2),                       
                            BorrowDate = reader.GetDateTime(3),                   
                            ReturnDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4)  
                        });
                    }
                }
            }

            return detailedRecords;
        }
        
         
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

        
public async Task<int> AddAsync(BorrowRecord borrowRecord)
{
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var transaction = await connection.BeginTransactionAsync())
        {
            try
            {
                 
                var checkAmountQuery = "SELECT Amount FROM Books WHERE Id = @BookId";
                var commandCheckAmount = new NpgsqlCommand(checkAmountQuery, connection, transaction);
                commandCheckAmount.Parameters.AddWithValue("@BookId", borrowRecord.BookId);

                var amount = await commandCheckAmount.ExecuteScalarAsync();
                
                 
                if (amount == null || Convert.ToInt32(amount) <= 0)
                {
                    throw new Exception("Книга недоступна для выдачи. Остаток экземпляров равен 0.");
                }
                
                
                var updateBookQuery = @"
                    UPDATE Books
                    SET Amount = Amount - 1
                    WHERE Id = @BookId AND Amount > 0
                    RETURNING Amount";

                var commandUpdateBook = new NpgsqlCommand(updateBookQuery, connection, transaction);
                commandUpdateBook.Parameters.AddWithValue("@BookId", borrowRecord.BookId);

                 
                var updatedAmount = await commandUpdateBook.ExecuteScalarAsync();
                if (updatedAmount == null)
                {
                    throw new Exception("Книга недоступна для выдачи (количество экземпляров = 0).");
                }

                 
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

       
public async Task UpdateAsync(BorrowRecord borrowRecord)
{
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var transaction = await connection.BeginTransactionAsync())
        {
            try
            {
                 
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

                 
                var updateBookQuery = @"
                    UPDATE Books
                    SET Amount = Amount + 1
                    WHERE Id = @BookId";

                var commandUpdateBook = new NpgsqlCommand(updateBookQuery, connection, transaction);
                commandUpdateBook.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
                await commandUpdateBook.ExecuteNonQueryAsync();

                 
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
                c.Username AS ClientName,      
                b.Title AS BookTitle,           
                br.BorrowDate AS BorrowDate,    
                br.ReturnDate AS ReturnDate     
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
                            Id = reader.GetInt32(0),                               
                            ClientName = reader.GetString(1),                     
                            BookTitle = reader.GetString(2),                       
                            BorrowDate = reader.GetDateTime(3),                    
                            ReturnDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4)  
                        });
                    }
                }
            }

            return detailedRecords;
        }

        
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