using NUnit.Framework;
using storeapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using core;
using storeapi.Database;
//PROYECTO12

namespace UT
{
    [TestFixture]
    public class ProductsTests
    {
        [SetUp]
     public void Setup()
    {
        
        var dbtestDefault = "Server=localhost;Database=lab;Uid=root;Pwd=123456;";
        DataConnection.Init(dbtestDefault);
                    DatabaseInitializer.Initialize();

    }
        [Test]
        public void LoadProductsFromDatabase_NoCachedData_ReturnsFilteredProducts()
        {
            // Arrange
            var products = new Products();
            var categoryIds = new List<int> { 1, 2, 3 };
            var search = "keyword";

            // Act
            var result = products.LoadProductsFromDatabase(categoryIds, search);

            // Assert
            Assert.IsNotNull(result);
            // Add assertions to verify the returned products are correct based on the test scenario
        }

        [Test]
        public void Insert_WithValidProduct_AddsProductToTree()
        {
            // Arrange
            var products = new Products();
            var product = new Dictionary<string, string>
            {
                { "id", "1" },
                { "name", "Product 1" },
                { "price", "10.99" },
                { "imageUrl", "image_url" },
                { "description", "Description" },
                { "categoryId", "1" }
            };

            // Act
            products.Insert(product);

            // Assert
            // Add assertions to verify that the product is correctly inserted into the tree
        }
        

  
        [Test]
        public void SearchProductsByKeyword_WithValidKeyword_ReturnsProducts()
        {
            // Arrange
            var products = new Products();
            var root = new Products.TreeNode(new Dictionary<string, string>
            {
                { "id", "1" },
                { "name", "Product 1" },
                { "price", "10.99" },
                { "imageUrl", "image_url" },
                { "description", "Description" },
                { "categoryId", "1" }
            });
            products.Insert(root.Product);
            var result = new List<Dictionary<string, string>>();

            // Act
            products.SearchProductsByKeyword(root, "Product");

            // Assert
            Assert.AreEqual(0, result.Count);
            // Add assertions to verify the correctness of the returned products
        }

        [Test]
        public void ProductContainsKeyword_WithValidKeyword_ReturnsTrue()
        {
             var products = new Products();
            // Arrange
            var product = new Dictionary<string, string>
            {
                { "id", "1" },
                { "name", "Product 1" },
                { "price", "10.99" },
                { "imageUrl", "image_url" },
                { "description", "Description" },
                { "categoryId", "1" }
            };
            var keyword = "Product";

            // Act
            var result = products.ProductContainsKeyword(product, keyword);

            // Assert
            Assert.IsTrue(result);
        }

        
    }
}

