using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public enum OrderStatus
    {
        Created = 1,
        Confirmed = 2,
        Processing = 3,
        Delivery = 4,
        Completed = 5,

        [Display(Name = "Cancelled by Customer")]
        CancelledByCustomer = 6,

        Cancelled = 7
    }
}
