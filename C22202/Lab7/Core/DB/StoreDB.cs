using System;
using System.Data.Common;
using System.IO.Compression;
using MySqlConnector;
using ShopApi.Models;

namespace ShopApi.db;
public sealed class StoreDB
{
    public StoreDB(){}

    public static void CreateMysql()
    {
        var products = new List<Product>();

        for (int i = 1; i <= 30; i++)
        {
            products.Add(new Product
            {
                name = $"Product {i}",
                imgSource = $"producto.jpg",
                price = 10.99m * i,
                description = $"Description of Product {i}",
                id = i
            });
        }

        string connectionString = "Server=localhost;Port=3306;Database=mysql;Uid=root;Pwd=123456;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Create the products table if it does not exist
            string createTableQuery = @"
                DROP DATABASE IF EXISTS store;
                CREATE DATABASE store;
                use store;
                
                CREATE TABLE IF NOT EXISTS products (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100),
                    price DECIMAL(10, 2),
                    imgSource VARCHAR(255)
                );
                
                CREATE TABLE IF NOT EXISTS sales (
                    sale_id INT AUTO_INCREMENT PRIMARY KEY,
                    purchase_date DATETIME NOT NULL,
                    total DECIMAL(10, 2) NOT NULL,
                    payment_method ENUM('0', '1'),
                    purchase_number VARCHAR(50) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS sale_product (
                    sale_id INT,
                    product_id INT,
                    price DECIMAL(10, 2),
                    PRIMARY KEY (sale_id, product_id),
                    FOREIGN KEY (sale_id) REFERENCES sales(sale_id),
                    FOREIGN KEY (product_id) REFERENCES products(id)
                );";


            using (var command = new MySqlCommand(createTableQuery, connection))
            {
                int result = command.ExecuteNonQuery();
                bool dbNoCreated = result < 0;
                if(dbNoCreated)
                    throw new Exception("Error creating the bd");
            }

            // Begin a transaction
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Insert 30 products into the table
                    foreach(Product product in products)
                    {
                        string productName = product.name;
                        string productImage = product.imgSource;
                        decimal productPrice = product.price;

                        string insertProductQuery = @"
                            INSERT INTO products (name, price, imgSource)
                            VALUES (@name, @price, @imgSource);";

                        using (var insertCommand = new MySqlCommand(insertProductQuery, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@name", productName);
                            insertCommand.Parameters.AddWithValue("@price", productPrice);
                            insertCommand.Parameters.AddWithValue("@imgSource", productImage);
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction if all inserts are successful
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback the transaction if an error occurs
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}