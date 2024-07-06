using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using storeapi.Database;
using storeapi.Models;
//PROYECTO

namespace storeapi.Bussisnes
{
    public sealed class StoreLogic
    {
        public StoreLogic()
        {
        }
        private static Random random = new Random();
        private static string lastPurchaseNumber;

        public string PurchaseNumber { get; private set; }

        public async Task<Sale> PurchaseAsync(Cart cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");

            if (cart.ProductIds == null || cart.ProductIds.Count == 0)
                throw new ArgumentException("Cart must contain at least one product.", nameof(cart.ProductIds));

            AddressValidator addressValidator = new AddressValidator();
            addressValidator.Validate(cart.Address);

            var products = Store.Instance.Products;
            var taxPercentage = Store.Instance.TaxPercentage;

            IEnumerable<Product> matchingProducts = products.Where(p => cart.ProductIds.Contains(p.id.ToString())).ToList();

            List<Product> shadowCopyProducts = matchingProducts.Select(p => (Product)p.Clone()).ToList();

            foreach (var product in shadowCopyProducts)
            {
                product.Price *= 1 + (decimal)13 / 100;
            }

            decimal purchaseAmount = shadowCopyProducts.Sum(p => p.Price);

            if (string.IsNullOrEmpty(lastPurchaseNumber))
            {
                lastPurchaseNumber = GenerateNextPurchaseNumber();
            }

            PurchaseNumber = lastPurchaseNumber;

            PaymentMethods.Type paymentMethodType = cart.PaymentMethod;

            // Validar si el método de pago está activo
            var paymentMethod = PaymentMethods.Find(paymentMethodType);
            if (!paymentMethod.IsActive)
            {
                throw new InvalidOperationException($"El método de pago {paymentMethodType} está desactivado.");
            }

            var sale = new Sale(shadowCopyProducts, cart.Address, purchaseAmount, paymentMethodType);

            CartSave cartSave = new CartSave();
            await cartSave.SaveSaleAndItemsToDatabaseAsync(sale);

            return sale;
        }

        public static string GenerateNextPurchaseNumber()
        {
            long ticks = DateTime.Now.Ticks;
            int randomNumber = random.Next();
            int uniqueNumber = (int)(ticks & 0xFFFFFFFF) ^ randomNumber;
            return uniqueNumber.ToString();
        }
    }
}
