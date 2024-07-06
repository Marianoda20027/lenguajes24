using System;
//PROYECTO1

namespace storeapi.Models
{
    public abstract class PaymentMethods
    {
        public enum Type
        {
            CASH = 0,
            SINPE = 1
        }

        public Type PaymentType { get; set; }
        public bool IsActive { get; set; }

        protected PaymentMethods(Type paymentType, bool isActive = true)
        {
            PaymentType = paymentType;
            IsActive = isActive;
        }

        private static readonly Sinpe sinpe = new Sinpe();
        private static readonly Cash cash = new Cash();

        public static PaymentMethods Find(Type type)
        {
            switch (type)
            {
                case Type.CASH:
                    return cash;
                case Type.SINPE:
                    return sinpe;
                default:
                    throw new NotImplementedException("Payment method not implemented");
            }
        }

        public static PaymentMethods SetPaymentType(Type type)
        {
            return Find(type);
        }

        public static PaymentMethods LoadFromDatabase(int id, string name, bool isActive)
        {
            var paymentType = (Type)id;
            switch (paymentType)
            {
                case Type.CASH:
                    return new Cash { PaymentType = paymentType, IsActive = isActive };
                case Type.SINPE:
                    return new Sinpe { PaymentType = paymentType, IsActive = isActive };
                default:
                    throw new NotImplementedException("Payment method not implemented");
            }
        }
    }

    public sealed class Sinpe : PaymentMethods
    {
        public Sinpe() : base(Type.SINPE) { }
    }

    public sealed class Cash : PaymentMethods
    {
        public Cash() : base(Type.CASH) { }
    }
}

