using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class ProductDescriptionItem
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }

        public ProductDescriptionField Field { get; set; }
        public int FieldId { get; set; }
    }
}
