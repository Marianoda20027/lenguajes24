using Store_API.Database;
using Store_API.Models;

namespace Store_API.Business
{
    public sealed class StoreLogic
    {
        private DB_API dbAPI = new DB_API();

        public string Purchase(Cart cart)
        {
            var products = Store.Instance.Products;
            var taxPercentage = Store.Instance.TaxPercentage;

            // Encontrar productos basados en los IDs en el carrito
            IEnumerable<Product> matchingProducts = products.Where(p => cart.ProductIds.Contains(p.Id)).ToList();

            decimal purchaseAmount = 0;
            foreach (var product in matchingProducts)
            {
                purchaseAmount += product.Price;
            }

            PaymentMethods.Type paymentMethodType = cart.PaymentMethod;

            string purchaseNumber = GeneratePurchaseNumber();

            var sale = new Sale(matchingProducts, cart.Address, purchaseAmount, paymentMethodType, purchaseNumber);

            sale.PurchaseNumber = purchaseNumber;
            dbAPI.InsertSale(sale);

            return purchaseNumber;
        }

        private string GeneratePurchaseNumber()
        {
            Random random = new Random();
            string letters = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            string numbers = random.Next(100, 999).ToString();

            return $"{letters}{numbers}";
        }
    }
}