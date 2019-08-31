using System;

namespace onlineShop.Models
{
    public class ProductComment
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public AppUser Customer { get; set; }

        // for users leaving comment/review without logging in
        public string GuestUserName { get; set; }

        public string Text { get; set; }
        public DateTime DateAdded { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int RatingValue { get; set; }
        public bool IsVerifiedPurchase { get; set; }

        public bool IsPostedByGuest { get { return this.Customer == null; } }
        public bool IsPublished { get; set; }
    }
}
