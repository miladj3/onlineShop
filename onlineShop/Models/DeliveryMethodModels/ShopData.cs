using onlineShop.Contracts;
using onlineShop.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShop.Models
{
    public class StoreData
    {
        public StoreData()
        {
        }

        [NotMapped]
        public DeliveryMethodType deliveryMethod { get; set; }

        [Key]
        public int OrderId { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Store)]
        public string StoreCode { get; set; }


    }
}
