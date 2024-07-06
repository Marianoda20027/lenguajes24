using NUnit.Framework;
using storeapi.Bussisnes;
using storeapi.Database;
using storeapi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core;
//PROYECTO12

namespace UT
{
    [TestFixture]
    public class StoreLogicTests
    {
        [SetUp]
        public void Setup()
        {
            var dbtestDefault = "Server=localhost;Database=lab;Uid=root;Pwd=123456;";
            DataConnection.Init(dbtestDefault);
            DatabaseInitializer.Initialize();
        }
        [Test]
        public async Task Purchase_ValidCart_ReturnsSale()
        {

            // Arrange
            var storeLogic = new StoreLogic();
            var cart = new Cart
            {
                ProductIds = new List<string> { "1", "2", "3" },
                Address = "123 Main St",
                PaymentMethod = PaymentMethods.Type.CASH
            };

            // Act
            var sale = await storeLogic.PurchaseAsync(cart);

            // Assert
            Assert.IsNotNull(sale);
            Assert.IsNotEmpty(sale.Products);
            Assert.AreEqual("123 Main St", sale.Address);
            Assert.AreEqual(PaymentMethods.Type.CASH, sale.PaymentMethod);
            Assert.Greater(sale.PurchaseNumber.Length, 0, "La longitud del número de compra debe ser mayor que cero.");

            foreach (var product in sale.Products)
            {
                Assert.Greater(product.Price, 0);
                decimal taxPercentage = Store.Instance.TaxPercentage;

                decimal expectedPriceWithTax = Math.Round(product.Price * (1 + 13 / 100), 2);

                decimal tolerance = 0.1m;

                Assert.IsTrue(Math.Abs(expectedPriceWithTax - product.Price) < tolerance, $"La diferencia entre el precio con impuestos ({expectedPriceWithTax}) y el precio original ({product.Price}) no está dentro de la tolerancia ({tolerance}).");



            }


        }

        [Test]
        public void Purchase_NullCart_ThrowsArgumentNullException()
        {
            // Arrange
            var storeLogic = new StoreLogic();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await storeLogic.PurchaseAsync(null));
        }

        [Test]
        public void Purchase_EmptyProductIds_ThrowsArgumentException()
        {
            // Arrange
            var storeLogic = new StoreLogic();
            var cart = new Cart
            {
                ProductIds = new List<string>(), // Empty product IDs
                Address = "123 Main St",
                PaymentMethod = PaymentMethods.Type.CASH
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await storeLogic.PurchaseAsync(cart));
        }

        [Test]
        public void Purchase_NullAddress_ThrowsArgumentException()
        {
            // Arrange
            var storeLogic = new StoreLogic();
            var cart = new Cart
            {
                ProductIds = new List<string> { "1", "2", "3" },
                Address = null, // Null address
                PaymentMethod = PaymentMethods.Type.CASH
            };

            // Act & Assert
            Assert.ThrowsAsync<System.NullReferenceException>(async () => await storeLogic.PurchaseAsync(cart));
        }
    }
}
