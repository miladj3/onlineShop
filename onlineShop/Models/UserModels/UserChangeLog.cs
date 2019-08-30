namespace onlineShop.Models
{
    public class UserChangeLog
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int ChangeLogId { get; set; }
        public ChangeLog ChangeLog { get; set; }
    }
}
