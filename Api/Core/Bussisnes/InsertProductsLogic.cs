//LAB13
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using storeapi.Database;
using storeapi.Models;
using core;
using MySqlConnector;
//PROYECTO

namespace storeapi.Business
{
    public class InsertProductsLogic
    {
        private readonly IMemoryCache _cache;

        public delegate void InsertProductDelegate(Product product, MySqlConnection connection, MySqlTransaction transaction);

        private readonly InsertProductDelegate _insertProductDelegate;

        public InsertProductsLogic(IMemoryCache cache, InsertProductDelegate insertProductDelegate)
        {
            _cache = cache;
            _insertProductDelegate = insertProductDelegate;
        }

        public List<Product> InsertProduct(Product product)
        {
            ValidateProduct(product);

            if (!_cache.TryGetValue("Products", out List<Product> products))
            {
                products = Store.LoadProductsFromDatabase().ToList();
                _cache.Set("Products", products);
            }

            using (var connection = new MySqlConnection(DataConnection.Instance.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        _insertProductDelegate(product, connection, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error inserting product into database: {ex.Message}");
                    }
                }
            }

            products.Add(product);
            _cache.Set("Products", products);

            return products;
        }

        private void ValidateProduct(Product product)
        {
            if (product == null) throw new ArgumentException("El producto no puede ser nulo.");
            if (string.IsNullOrWhiteSpace(product.Name)) throw new ArgumentException("El nombre del producto no puede estar vacío o ser nulo.", nameof(product.Name));
            if (product.Price <= 0) throw new ArgumentOutOfRangeException(nameof(product.Price), "El precio del producto debe ser mayor que cero.");
            if (string.IsNullOrWhiteSpace(product.ImageUrl)) throw new ArgumentException("La URL de la imagen del producto no puede estar vacía o ser nula.", nameof(product.ImageUrl));
            if (string.IsNullOrWhiteSpace(product.Description)) throw new ArgumentException("La descripción del producto no puede estar vacía o ser nula.", nameof(product.Description));
        }
    }
}
