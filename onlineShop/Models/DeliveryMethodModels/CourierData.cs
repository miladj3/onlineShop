using onlineShop.Contracts;
using onlineShop.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShop.Models
{
    public class CourierData
    {
        public CourierData()
        {
        }

        [NotMapped]
        public DeliveryMethodType deliveryMethod { get; set; }

        [Key]
        public int OrderId { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Street { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Building { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Appartment { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string PostalCode { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string City { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Country { get; set; }
    }
}
