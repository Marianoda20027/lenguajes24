using System;
using MySqlConnector;
using core;

namespace storeapi.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                connection.Open();

                // Crear la tabla de productos si no existe
                string createProductsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS products (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        price DECIMAL(10, 2) NOT NULL,
                        description VARCHAR(255) NOT NULL,
                        image VARCHAR(255) NOT NULL,
                        category INT NOT NULL
                    )";

                using (var command = new MySqlCommand(createProductsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Crear la tabla de compras si no existe
                string createComprasTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Compras (
                        total DECIMAL(10, 2) NOT NULL,
                        date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        purchaseNumber VARCHAR(255) NOT NULL,
                        Paymethod INT,
                        ProductsId VARCHAR(100),
                        PRIMARY KEY (purchaseNumber)
                    );";

                using (var command = new MySqlCommand(createComprasTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Crear la tabla de categorías si no existe
                string createCategoriesTableQuery = @"
                    CREATE TABLE IF NOT EXISTS categories (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        name VARCHAR(100) NOT NULL
                    )";

                using (var command = new MySqlCommand(createCategoriesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Crear la tabla de items si no existe
                string createItemsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Items (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        ProductId INT,
                        PurchaseNumber VARCHAR(255),
                        Address VARCHAR(255),
                        Price DECIMAL(10, 2)
                    );";

                using (var command = new MySqlCommand(createItemsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
