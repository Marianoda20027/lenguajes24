
using System;
using System.Data.Common;
using System.IO.Compression;
using MySqlConnector;


namespace TodoApi.Database {
public sealed class SaleDB
{
    public void save(Sale sale)
    {
        using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=mysql;Port=3306;Uid=root;Pwd=123456;"))
        {
            connection.Open();

            string insertQuery = @"
            use store;

                   INSERT INTO sales (productIds, purchase_date, total, payment_method, purchase_number)
                    VALUES (@productIds, @purchase_date, @total, @payment_method, @purchase_number);";

        
                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@productIds", string.Join(",", sale.Products));
                    command.Parameters.AddWithValue("@purchase_date", DateTime.Now);
                    command.Parameters.AddWithValue("@total", sale.Amount);
                    command.Parameters.AddWithValue("@payment_method", sale.PaymentMethod);
                    command.Parameters.AddWithValue("@purchase_number", sale.PurchaseNumber);
                    command.ExecuteNonQuery();
                }
        }
    }
}}