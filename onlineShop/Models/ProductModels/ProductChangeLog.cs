namespace onlineShop.Models
{
    public class ProductChangeLog
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ChangeLogId { get; set; }
        public ChangeLog ChangeLog { get; set; }
    }
}
