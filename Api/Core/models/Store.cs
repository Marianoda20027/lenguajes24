using System;
using System.Collections.Generic;
using storeapi.Database;

namespace storeapi.Models
{
    public sealed class Store
    {
        public IEnumerable<Product> Products { get; private set; }
        public int TaxPercentage { get; private set; }

        private Store(IEnumerable<Product> products, int taxPercentage)
        {
            Products = products;
            TaxPercentage = taxPercentage;
        }

        public static Store Instance => CreateInstance();

        private static Store CreateInstance()
        {
            IEnumerable<Product> products = LoadProductsFromDatabase();
            int taxPercentage = 13;
            return new Store(products, taxPercentage);
        }

        public static IEnumerable<Product> LoadProductsFromDatabase()
        {
            List<string[]> productData = StoreDB.RetrieveDatabaseInfo();
            List<Product> products = new List<Product>();
            Categories categories = new Categories();

            foreach (string[] row in productData)
            {
                if (ValidateProductRow(row))
                {
                    int id = int.Parse(row[0]);
                    string name = row[1];
                    decimal price = decimal.Parse(row[2]);
                    string imageUrl = row[3];
                    string description = row[4];
                    int categoryId = int.Parse(row[5]);

                    Category category = categories.GetCategoryById(categoryId);

                    Product product = new Product
                    {
                        id = id,
                        Name = name,
                        Price = price,
                        ImageUrl = imageUrl,
                        Description = description,
                        Category = category
                    };

                    products.Add(product);
                }
            }

            return products;
        }

        private static bool ValidateProductRow(string[] row)
        {
            if (row == null || row.Length < 6)
            {
                throw new ArgumentException("Invalid product data row: Insufficient columns.");
            }

            if (!int.TryParse(row[0], out _))
            {
                throw new ArgumentException("Invalid product ID.");
            }

            if (string.IsNullOrWhiteSpace(row[1]))
            {
                throw new ArgumentException("Product name is null or empty.");
            }

            if (!decimal.TryParse(row[2], out _))
            {
                throw new ArgumentException("Invalid product price.");
            }

            if (string.IsNullOrWhiteSpace(row[3]))
            {
                throw new ArgumentException("Image URL is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(row[4]))
            {
                throw new ArgumentException("Product description is null or empty.");
            }

            if (!int.TryParse(row[5], out _))
            {
                throw new ArgumentException("Invalid category ID.");
            }

            return true;
        }
    }
}
