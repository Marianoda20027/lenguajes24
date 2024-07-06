using System;
using System.Collections.Generic;
using MySqlConnector;
using storeapi.Models;
using core;
//PROYECTO

namespace storeapi.Database
{
    public sealed class StoreDB
    {
        // Define the delegate for inserting a product
        public delegate void InsertProductDelegate(Product product, MySqlConnection connection, MySqlTransaction transaction);

        public static void CreateMysql()
        {
            var categories = new Categories();
            var products = new List<Product>();
            Random random = new Random();

            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                connection.Open();

                // Crear la tabla si no existe
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS products (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        name VARCHAR(100) not null,
                        price DECIMAL(10, 2) not null,
                        image TEXT not null,
                        description VARCHAR(255) not null,
                        category INT not null
                    )";

                using (var createTableCommand = new MySqlCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                // Verificar si ya existen productos en la tabla
                string checkProductsQuery = "SELECT COUNT(*) FROM products";
                using (var checkProductsCommand = new MySqlCommand(checkProductsQuery, connection))
                {
                    int productCount = Convert.ToInt32(checkProductsCommand.ExecuteScalar());
                    if (productCount > 0)
                    {
                        return;
                    }
                }

                // Generar productos
                string[] randomWords = { "amazing", "awesome", "fantastic", "incredible", "superb", "excellent", "wonderful", "marvelous", "brilliant", "fabulous" };
                string[] productNames = { "Gizmo", "Widget", "Contraption", "Gadget", "Appliance", "Device", "Tool", "Instrument", "Machine", "Equipment" };

                for (int i = 1; i <= 14; i++)
                {
                    Category randomCategory = GetRandomCategory(categories);
                    int randomIndex = random.Next(0, categories.ListCategories.Count);

                    string description = $"Description of Product {i}: ";
                    for (int j = 0; j < 1; j++)
                    {
                        int innerRandomWordIndex = random.Next(0, randomWords.Length);
                        description += randomWords[innerRandomWordIndex] + " ";
                    }

                    int randomWordIndex = random.Next(0, randomWords.Length);
                    int randomNameIndex = random.Next(0, productNames.Length);
                    string productName = $"{productNames[randomNameIndex]} {randomWords[randomWordIndex]}";

                    products.Add(new Product
                    {
                        Name = productName,
                        ImageUrl = $"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSlgv-oyHOyGGAa0U9W524JKA361U4t22Z7oQ&usqp=CAU",
                        Price = 10.99m * i,
                        Description = description.Trim(),
                        Category = randomCategory
                    });
                }

                if (products.Count == 0)
                {
                    throw new ArgumentException("La lista de productos no puede estar vacía.", nameof(products));
                }

                InsertProducts(products, InsertProduct);
            }
        }

        // Method to insert products using a delegate
        public static void InsertProducts(List<Product> products, InsertProductDelegate insertProductDelegate)
        {
            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteProductsQuery = "DELETE FROM products";
                        using (var deleteCommand = new MySqlCommand(deleteProductsQuery, connection, transaction))
                        {
                            deleteCommand.ExecuteNonQuery();
                        }

                        foreach (Product product in products)
                        {
                            ValidateProductForInsert(product);
                            insertProductDelegate(product, connection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error inserting products into database: {ex.Message}");
                    }
                }
            }
        }

        // Delegate method to insert a product
        public static void InsertProduct(Product product, MySqlConnection connection, MySqlTransaction transaction)
        {
            string insertProductQuery = @"
                INSERT INTO products (name, price, description, image, category)
                VALUES (@name, @price, @description, @image, @category)";

            using (var insertCommand = new MySqlCommand(insertProductQuery, connection, transaction))
            {
                insertCommand.Parameters.AddWithValue("@name", product.Name);
                insertCommand.Parameters.AddWithValue("@price", product.Price);
                insertCommand.Parameters.AddWithValue("@description", product.Description);
                insertCommand.Parameters.AddWithValue("@image", product.ImageUrl);
                insertCommand.Parameters.AddWithValue("@category", product.Category.Id);
                insertCommand.ExecuteNonQuery();
            }
        }

        public static List<string[]> RetrieveDatabaseInfo()
        {
            List<string[]> databaseInfo = new List<string[]>();
            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM products";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int fieldCount = reader.FieldCount;
                            string[] row = new string[fieldCount];
                            for (int i = 0; i < fieldCount; i++)
                            {
                                row[i] = reader.GetValue(i).ToString();
                            }
                            databaseInfo.Add(row);
                        }
                    }
                }
            }

            return databaseInfo;
        }

        private static Category GetRandomCategory(Categories categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories), "La instancia de 'categories' no puede ser nula.");
            }

            List<Category> categoryList = categories.ListCategories;

            if (categoryList == null || categoryList.Count == 0)
            {
                throw new ArgumentException("La lista de categorías está vacía o es nula.");
            }

            Random random = new Random();
            int index = random.Next(0, categoryList.Count);

            return categoryList[index];
        }

        private static void ValidateProductForInsert(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "El producto no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("El nombre del producto no puede ser nulo o vacío.", nameof(product.Name));
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("El precio del producto no puede ser negativo.", nameof(product.Price));
            }
        }
    }
}



