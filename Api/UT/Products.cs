// File: UT/InsertProductsLogicTests.cs
using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;
using storeapi.Business;
using storeapi.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using core;
using storeapi.Database;
//PROYECTO12

namespace UT
{
    [TestFixture]
    public class InsertProductsLogicTests : IDisposable
    {
        private IMemoryCache _memoryCache;
        private InsertProductsLogic _insertProductsLogic;

        [SetUp]
        public void Setup()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            var dbtestDefault = "Server=localhost;Database=lab;Uid=root;Pwd=123456;";
            DataConnection.Init(dbtestDefault);
            DatabaseInitializer.Initialize();
            _insertProductsLogic = new InsertProductsLogic(_memoryCache, StoreDB.InsertProduct);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Dispose();
        }

        [Test]
        public void InsertProduct_NewProduct_AddsToCache()
        {
            // Arrange
            var newProduct = new Product
            {
                id = 1,
                Name = "Awesome",
                Price = 99,
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSlgv-oyHOyGGAa0U9W524JKA361U4t22Z7oQ&usqp=CAU",
                Description = "New Product Description",
                Category = new Category { _id = 1, _name = "Electronica" }
            };

            // Act
            var result = _insertProductsLogic.InsertProduct(newProduct);

            // Assert
            Assert.IsTrue(result.Any(p => p.id == newProduct.id && p.Name == newProduct.Name));
            Assert.IsTrue(_memoryCache.TryGetValue("Products", out List<Product> cachedProducts));
            Assert.IsTrue(cachedProducts.Any(p => p.id == newProduct.id && p.Name == newProduct.Name));
        }

        [Test]
        public void InsertProduct_ExistingProduct_UpdatesInCache()
        {
            // Arrange
            var existingProduct = new Product
            {
                id = 1,
                Name = "Amazing Product",
                Price = 29,
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSlgv-oyHOyGGAa0U9W524JKA361U4t22Z7oQ&usqp=CAU",
                Description = "Big Product",
                Category = new Category { _id = 1, _name = "Electronica" }
            };
            _memoryCache.Set("Products", new List<Product> { existingProduct });

            var updatedProduct = new Product
            {
                id = 1,
                Name = "Amazing Product",
                Price = 29,
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSlgv-oyHOyGGAa0U9W524JKA361U4t22Z7oQ&usqp=CAU",
                Description = "Big Product",
                Category = new Category { _id = 1, _name = "Electronica" }
            };

            // Act
            var result = _insertProductsLogic.InsertProduct(updatedProduct);

            // Assert
            var productInCache = result.FirstOrDefault(p => p.id == updatedProduct.id);
            Assert.NotNull(productInCache);
            Assert.AreEqual(updatedProduct.Name, productInCache.Name);
            Assert.AreEqual(updatedProduct.Price, productInCache.Price);
            Assert.AreEqual(updatedProduct.ImageUrl, productInCache.ImageUrl);
            Assert.AreEqual(updatedProduct.Description, productInCache.Description);
            Assert.AreEqual(updatedProduct.Category._name, productInCache.Category._name);
        }

        private void InsertProductToList(Product product, List<Product> products)
        {
            var existingProduct = products.FirstOrDefault(p => p.id == product.id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Description = product.Description;
                existingProduct.Category = product.Category;
            }
            else
            {
                products.Add(product);
            }
        }

        public void Dispose()
        {
            _memoryCache?.Dispose();
        }
    }
}
