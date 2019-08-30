using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class Category
    {
        public Category()
        {
            this.Subcategories = new List<Subcategory>();
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public List<Subcategory> Subcategories { get; set; }
        public Department Department { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public FilePath Picture { get; set; }

    }
}
