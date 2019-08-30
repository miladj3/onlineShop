using System.Collections.Generic;

namespace onlineShop.Models
{
    public class ProductDescription
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<ProductDescriptionItem> ProductDescriptionItems { get; set; }
    }
}
