namespace onlineShop.Models
{
    public class OrderChangeLog
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ChangeLogId { get; set; }
        public ChangeLog ChangeLog { get; set; }
    }
}
