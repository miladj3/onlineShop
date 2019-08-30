using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShop.Models
{
    [NotMapped]
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Amount { get => this.Product.SalePrice * this.Quantity; }
    }
}
