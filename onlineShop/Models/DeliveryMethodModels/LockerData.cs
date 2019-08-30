using onlineShop.Contracts;
using onlineShop.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShop.Models
{
    public class LockerData
    {
        public LockerData()
        {
        }

        [NotMapped]
        public DeliveryMethodType deliveryMethod { get; set; }

        [Key]
        public int OrderId { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Locker)]
        public string LockerCode { get; set; }
    }
}
