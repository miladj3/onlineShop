using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public enum PaymentMethod
    {
        Cash = 0,
        [Display(Name = "Bank Transfer")]
        BankTransfer = 1
    }
}
