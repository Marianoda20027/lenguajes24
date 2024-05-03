using NUnit.Framework;
using Store_API.Business;
using Store_API.Models;

namespace UnitTests
{
    public class StoreLogicTests
    {
        private StoreLogic _storeLogic;

        [SetUp]
        public void Setup()
        {
            _storeLogic = new StoreLogic();
        }

        [Test]
        public void Purchase_WithEmptyCart_ThrowsArgumentException()
        {
            Cart cart = new Cart
            {
                ProductIds = new List<int>(),
                Address = "123 Main St",
                PaymentMethod = PaymentMethods.Type.SINPE
            };

            Assert.Throws<ArgumentException>(() => _storeLogic.Purchase(cart));
        }

        [Test]
        public void Purchase_WithMissingAddress_ThrowsArgumentException()
        {
            Cart cart = new Cart
            {
                ProductIds = new List<int> { 1, 2 },
                Address = "",
                PaymentMethod = PaymentMethods.Type.CASH
            };

            Assert.Throws<ArgumentException>(() => _storeLogic.Purchase(cart));
        }

        [Test]
        public void Purchase_HappyPath()
        {
            Cart cart = new Cart
            {
                ProductIds = new List<int> { 3, 4 },
                Address = "San José, Costa Rica",
                PaymentMethod = PaymentMethods.Type.CASH
            };

            string mockPurchaseNumber = "FGH678";
            DB_API mockDB = new DB_API();
            mockDB.InsertSale = sale =>
            {
                sale.PurchaseNumber = mockPurchaseNumber;
                return mockPurchaseNumber;
            };

            _storeLogic.dbAPI = mockDB;

            string purchaseNumber = _storeLogic.Purchase(cart);

            Assert.AreEqual(mockPurchaseNumber, purchaseNumber);
        }
    }
}