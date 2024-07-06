using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using storeapi.Models;
using core;

namespace storeapi.Database
{
    public sealed class CampannaDB
    {
        public CampannaDB() { }

        public static async Task CreateMysqlAsync()
        {
            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Campannas (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        ContenidoHtml TEXT NOT NULL,
                        estado BOOLEAN,
                        CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB";

                using (var createTableCommand = new MySqlCommand(createTableQuery, connection))
                {
                    await createTableCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task InsertCampannaAsync(Campanna campanna)
        {
            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string insertQuery = @"
                    INSERT INTO Campannas (ContenidoHtml, estado)
                    VALUES (@ContenidoHtml, @Estado)";

                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@ContenidoHtml", campanna.ContenidoHtml);
                    insertCommand.Parameters.AddWithValue("@Estado", campanna.Estado);
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task UpdateCampannaEstadoAsync(int id, bool estado)
        {
            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string updateQuery = @"
                    UPDATE Campannas
                    SET estado = @Estado
                    WHERE Id = @Id";

                using (var updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Estado", estado);
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task<IEnumerable<Campanna>> LoadCampannasFromDatabaseAsync()
        {
            var campannas = new List<Campanna>();

            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT Id, ContenidoHtml, CreatedAt, estado FROM Campannas WHERE estado = true";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var campanna = new Campanna
                            {
                                Id = reader.GetInt32(0),
                                ContenidoHtml = reader.GetString(1),
                                CreatedAt = reader.GetDateTime(2),
                                Estado = reader.GetBoolean(3)
                            };
                            campannas.Add(campanna);
                        }
                    }
                }
            }

            return campannas;
        }

        public static async Task<Campanna> GetCampannaByIdAsync(int id)
        {
            Campanna campanna = null;

            if (id <= 0)
            {
                throw new ArgumentException("ID must be a positive number.");
            }

            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT Id, ContenidoHtml, CreatedAt, estado FROM Campannas WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            campanna = new Campanna
                            {
                                Id = reader.GetInt32(0),
                                ContenidoHtml = reader.GetString(1),
                                CreatedAt = reader.GetDateTime(2),
                                Estado = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }

            return campanna;
        }
    }
}


