using System;
using System.Collections.Generic;
using storeapi.Bussisnes;

namespace storeapi.Models
{
    public sealed class Sale
    {
        public IEnumerable<Product> Products { get; }
        public string Address { get; }
        public decimal Amount { get; }
        public PaymentMethods.Type PaymentMethod { get; set; }
        public string PurchaseNumber { get; }

        public Sale(IEnumerable<Product> products, string address, decimal amount, PaymentMethods.Type paymentMethod)
        {
            if (products == null || !products.Any())
            {
                throw new ArgumentException("Products collection must not be null or empty.", nameof(products));
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address must not be null or empty.", nameof(address));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be a positive decimal value.", nameof(amount));
            }

            Products = products;
            Address = address;
            Amount = amount;
            PaymentMethod = paymentMethod;
            PurchaseNumber = StoreLogic.GenerateNextPurchaseNumber();
        }
    }
}