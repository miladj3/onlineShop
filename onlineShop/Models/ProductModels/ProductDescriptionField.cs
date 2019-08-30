using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class ProductDescriptionField
    {
        [Key]
        public int Id { get; set; }
        public Subcategory Subcategory { get; set; }
        public int? SubcategoryId { get; set; }
        public string Name { get; set; }
        public bool DisplayInItemPreview { get; set; }
        public int DisplayOrderId { get; set; }
    }
}
