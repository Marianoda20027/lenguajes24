using storeapi.Database;
namespace storeapi.Models
{
    public sealed class Payment
    {
        public PaymentMethods Cash { get; private set; }
        public PaymentMethods Sinpe { get; private set; }

        public static readonly Payment Instance = new Payment();

        private Payment()
        {
            var methods = LoadPaymentMethodsFromDatabase();
            Cash = methods.ContainsKey(PaymentMethods.Type.CASH) ? methods[PaymentMethods.Type.CASH] : null;
            Sinpe = methods.ContainsKey(PaymentMethods.Type.SINPE) ? methods[PaymentMethods.Type.SINPE] : null;

            if (Cash == null || Sinpe == null)
            {
                throw new InvalidOperationException("No se encontraron ambos métodos de pago necesarios.");
            }
        }

        private Dictionary<PaymentMethods.Type, PaymentMethods> LoadPaymentMethodsFromDatabase()
        {
            var paymentMethods = new Dictionary<PaymentMethods.Type, PaymentMethods>();
            var data = PaymentDB.RetrievePaymentMethods();

            foreach (var methodData in data)
            {
                int id = int.Parse(methodData[0]);
                string name = methodData[1];
                bool isActive = methodData[2] == "true"; // Asegurarse de que el valor se convierta a booleano

                var method = PaymentMethods.LoadFromDatabase(id, name, isActive);
                paymentMethods.Add((PaymentMethods.Type)id, method);
            }

            return paymentMethods;
        }
    }
}
