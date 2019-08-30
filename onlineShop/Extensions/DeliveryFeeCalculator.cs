using onlineShop.Models;

namespace onlineShop.Extensions
{
    public static class DeliveryFeeCalculator
    {
        private const double DeliveryMethodLockerFee = 12;
        private const double DeliveryMethodStoreFee = 0;
        private const double DeliveryMethodCourierFee = 25;

        public static double Calculate(DeliveryMethodType deliveryMethod)
        {
            switch (deliveryMethod)
            {
                case DeliveryMethodType.Courier:
                    return DeliveryMethodCourierFee;

                case DeliveryMethodType.Locker:
                    return DeliveryMethodLockerFee;

                case DeliveryMethodType.Store:
                    return DeliveryMethodStoreFee;

                default:
                    return 0;
            }
        }
    }
}
