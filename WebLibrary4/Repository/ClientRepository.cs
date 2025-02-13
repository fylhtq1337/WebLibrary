using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
using Npgsql;
using WebLibrary4.Models.Entities;
using WebLibrary4.Interfaces; // Интерфейс для клиента

namespace WebLibrary4.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public async Task<IEnumerable<Clients>> GetPaginatedClientsAsync(int skip, int take)
        {
            var clients = new List<Clients>();

            using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT * FROM Clients ORDER BY Id OFFSET @Skip LIMIT @Take";

            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Skip", skip);
            command.Parameters.AddWithValue("@Take", take);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var client = new Clients
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                    Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString(reader.GetOrdinal("Role"))
                };

                clients.Add(client);
            }

            return clients;
        }
        public async Task<int> GetTotalClientsCountAsync()
        {
            using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT COUNT(*) FROM Clients";

            using var command = new Npgsql.NpgsqlCommand(query, connection);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        
        public async Task<IEnumerable<Clients>> SearchByNameAsync(string name)
        {
            var clients = new List<Clients>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                 
                var query = @"SELECT * FROM Clients WHERE Username ILIKE '%' || @Name || '%'";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clients.Add(new Clients
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return clients;
        }

        public async Task<IEnumerable<Clients>> GetAllAsync()
        {
            var clients = new List<Clients>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM Clients", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        clients.Add(new Clients
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Role = reader.GetString(3) // Так же читается роль
                        });
                    }
                }
            }

            return clients;
        }

        public async Task<Clients?> GetByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM Clients WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Clients
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Role = reader.GetString(3)
                        };
                    }
                }
            }

            return null; // Клиент с заданным Id не найден
        }

        public async Task<int> AddAsync(Clients client)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    @"INSERT INTO Clients (Username, Email, Role) 
                      VALUES (@Username, @Email, @Role) 
                      RETURNING Id", 
                    connection
                );

                command.Parameters.AddWithValue("@Username", client.Username);
                command.Parameters.AddWithValue("@Email", client.Email);
                command.Parameters.AddWithValue("@Role", client.Role);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task UpdateAsync(Clients client)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    @"UPDATE Clients 
                      SET Username = @Username, Email = @Email, Role = @Role 
                      WHERE Id = @Id", 
                    connection
                );

                command.Parameters.AddWithValue("@Id", client.Id);
                command.Parameters.AddWithValue("@Username", client.Username);
                command.Parameters.AddWithValue("@Email", client.Email);
                command.Parameters.AddWithValue("@Role", client.Role);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("DELETE FROM Clients WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                // Выполняем запрос и проверяем количество затронутых строк
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0; // Если удалена хотя бы одна строка, возвращаем true
            }
        }
    }
}