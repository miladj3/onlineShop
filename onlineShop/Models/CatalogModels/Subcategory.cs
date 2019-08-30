using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class Subcategory
    {
        public Subcategory()
        {
            this.DescriptionFields = new List<ProductDescriptionField>();
            this.Products = new List<Product>();
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public Category Category { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public virtual List<ProductDescriptionField> DescriptionFields { get; set; }

        public FilePath Picture { get; set; }
        public List<Product> Products { get; set; }

    }
}
