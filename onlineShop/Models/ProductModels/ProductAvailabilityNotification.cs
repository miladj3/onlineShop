using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models.ProductModels
{
    public class ProductAvailabilityNotification
    {
        public int Id { get; set; }

        // for existing (registered) users
        public string CustomerId { get; set; }
        public AppUser Customer { get; set; }


        // for guest users
        public string Email { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
