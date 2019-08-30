using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class DeliveryDetails
    {
        public DeliveryDetails()
        {
        }

        public DeliveryDetails(DeliveryMethodType deliveryMethod)
        {
            this.DeliveryMethodType = deliveryMethod;

            switch (deliveryMethod)
            {
                case DeliveryMethodType.Courier:

                    this.CourierData = new CourierData();
                    break;

                case DeliveryMethodType.Locker:

                    this.LockerData = new LockerData();
                    break;

                case DeliveryMethodType.Store:

                    this.StoreData = new StoreData();
                    break;
            }
        }

        public LockerData LockerData { get; set; }
        public StoreData StoreData { get; set; }
        public CourierData CourierData { get; set; }

        [Key]
        public int OrderId { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public DeliveryMethodType DeliveryMethodType { get; set; }

        public string Comment { get; set; }
    }
}
