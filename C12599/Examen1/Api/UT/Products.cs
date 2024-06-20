﻿// File: UT/InsertProductsLogicTests.cs
using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;
using storeapi.Business;
using storeapi.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using core;

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
     //       _insertProductsLogic = new InsertProductsLogic(_memoryCache, InsertProductToList);

            var dbtestDefault = "Server=localhost;Database=lab;Uid=root;Pwd=123456;";
            DataConnection.Init(dbtestDefault);
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
                Name = "New Product",
                Price = 99,
                ImageUrl = "http://example.com/image.png",
                Description = "New Product Description",
                Category = new Category { _id = 1, _name = "New Category" }
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
                Name = "Existing Product",
                Price = 19,
                ImageUrl = "http://example.com/image.png",
                Description = "Existing Product Description",
                Category = new Category { _id = 1, _name = "Existing Category" }
            };
            _memoryCache.Set("Products", new List<Product> { existingProduct });

            var updatedProduct = new Product
            {
                id = 1,
                Name = "Updated Product",
                Price = 29,
                ImageUrl = "http://example.com/image.png",
                Description = "Updated Product Description",
                Category = new Category { _id = 1, _name = "Updated Category" }
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
            Assert.AreEqual(updatedProduct.Category.Name, productInCache.Category.Name);
        }

        internal  void InsertProductToList(Product product, List<Product> products)
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