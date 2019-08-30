using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class CustomerAddress
    {
        public string CustomerId { get; set; }
        public AppUser Customer { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Appartment { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
