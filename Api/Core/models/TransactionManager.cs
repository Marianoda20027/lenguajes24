using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using storeapi.Database;

namespace storeapi.Models
{
    public class TransactionManager
    {
        public class TransactionRecord
        {
            public decimal TotalAmount { get; set; }
            public DateTime TransactionDate { get; set; }
            public string PurchaseNumber { get; set; }
            public PaymentMethods PaymentMethod { get; set; }
            public IEnumerable<string> Products { get; set; }
        }

        public static async Task<IEnumerable<TransactionRecord>> LoadTransactionsFromDayAsync(IEnumerable<string[]> transactionData)
        {
            List<TransactionRecord> transactions = new List<TransactionRecord>();

            foreach (string[] row in transactionData)
            {
                if (row.Length < 5) // Assuming there's product information in the transaction data
                {
                    continue;
                }

                if (decimal.TryParse(row[0], out decimal totalAmount) &&
                    DateTime.TryParse(row[1], out DateTime transactionDate) &&
                    int.TryParse(row[2], out int purchaseNumber) &&
                    Enum.TryParse(row[3], out PaymentMethods.Type paymentMethodType))
                {
                    // Simulate the asynchronous search for the payment method
                    PaymentMethods paymentMethod = await Task.FromResult(PaymentMethods.Find(paymentMethodType));

                    if (paymentMethod != null)
                    {
                        if (totalAmount <= 0)
                        {
                            throw new ArgumentException("El monto total debe ser mayor que cero.");
                        }

                        if (string.IsNullOrWhiteSpace(row[2]))
                        {
                            throw new ArgumentException("El número de compra no puede estar vacío.");
                        }

                        // Extract products from the row
                        IEnumerable<string> products = row[4..]; // Assuming products start from index 4

                        TransactionRecord transaction = new TransactionRecord
                        {
                            TotalAmount = totalAmount,
                            TransactionDate = transactionDate,
                            PurchaseNumber = purchaseNumber.ToString(),
                            PaymentMethod = paymentMethod,
                            Products = products
                        };

                        transactions.Add(transaction);
                    }
                    else
                    {
                        throw new InvalidOperationException($"No se encontró el método de pago correspondiente: {paymentMethodType}");
                    }
                }
            }

            return transactions;
        }
      public static async Task<IEnumerable<TransactionRecord>> LoadTransactionsFromWeekAsync(IEnumerable<string[]> transactionData)
{
    List<TransactionRecord> transactions = new List<TransactionRecord>();

    foreach (var row in transactionData)
    {
        if (row.Length < 5) continue;

        if (decimal.TryParse(row[0], out decimal totalAmount) &&
            DateTime.TryParse(row[1], out DateTime transactionDate) &&
            int.TryParse(row[2], out int purchaseNumber) 
           )
        {
            var transactionRecord = new TransactionRecord
            {
                TotalAmount = totalAmount,
                TransactionDate = transactionDate,
                PurchaseNumber = purchaseNumber.ToString(),
            };

            transactions.Add(transactionRecord);
        }
    }

    return transactions;
}

}
}
