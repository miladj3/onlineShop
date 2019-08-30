using onlineShop.Models;
using onlineShop.Validation;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class OrderDetailsDTO
    {
        public string Email { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public DeliveryMethodType DeliveryMethodType { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Street { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Building { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Appartment { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string City { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Courier)]
        public string Country { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Store)]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }

        [RequiredForDeliveryMethodType(DeliveryMethodType.Locker)]
        [Display(Name = "Parcel Locker Code")]
        public string ParcelLockerCode { get; set; }

        public string Comment { get; set; }
    }
}
