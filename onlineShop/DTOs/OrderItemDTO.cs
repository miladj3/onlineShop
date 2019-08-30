namespace onlineShop.DTOs
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public int Quantity { get; set; }
        public double Amount { get => this.Product.Price * this.Quantity ;  }
    }
}
