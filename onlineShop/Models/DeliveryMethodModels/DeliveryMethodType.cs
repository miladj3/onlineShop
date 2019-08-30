using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public enum DeliveryMethodType
    {
        [Display(Name = "Courier")]
        Courier = 0,
        [Display(Name = "Pick Up in Store")]
        Store = 1,
        [Display(Name = "Parcel Locker")]
        Locker = 2
    }
}
