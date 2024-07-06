using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using core;

namespace storeapi.Database
{


    public sealed class SalesDB
    {
        public SalesDB()
    {
     
    }

        public async Task<IEnumerable<string[]>> GetForWeekAsync(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("La fecha no puede ser DateTime.MinValue");
            }

             using (MySqlConnection connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                DateTime startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                string formattedStartOfWeek = startOfWeek.ToString("yyyy-MM-dd");
                string formattedEndOfWeek = endOfWeek.ToString("yyyy-MM-dd");

                string sql = @"
                    SELECT *
                    FROM Compras
                    WHERE date >= @startOfWeek AND date <= @endOfWeek";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@startOfWeek", formattedStartOfWeek);
                    command.Parameters.AddWithValue("@endOfWeek", formattedEndOfWeek);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var databaseInfo = new List<string[]>();

                        while (await reader.ReadAsync())
                        {
                            int fieldCount = reader.FieldCount;
                            string[] row = new string[fieldCount];

                            for (int i = 0; i < fieldCount; i++)
                            {
                                row[i] = reader.GetValue(i).ToString();
                            }

                            databaseInfo.Add(row);
                        }

                        return databaseInfo;
                    }
                }
            }
        }

        public async Task<IEnumerable<string[]>> GetForDayAsync(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("La fecha no puede ser DateTime.");
            }

            await using (MySqlConnection connection = new MySqlConnection(DataConnection.Instance.ConnectionString))

            {
                await connection.OpenAsync();

                string sql = @"
                    SELECT *
                    FROM Compras
                    WHERE DATE(date) = DATE(@date)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    // Asigna el parámetro @date en el formato adecuado
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var databaseInfo = new List<string[]>();

                        while (await reader.ReadAsync())
                        {
                            int fieldCount = reader.FieldCount;
                            string[] row = new string[fieldCount];

                            for (int i = 0; i < fieldCount; i++)
                            {
                                row[i] = reader.GetValue(i).ToString();
                            }

                            databaseInfo.Add(row);
                        }

                        return databaseInfo;
                    }
                }
            }
        }
    }
}