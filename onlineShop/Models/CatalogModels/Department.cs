using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class Department
    {
        public Department()
        {
            this.Categories = new List<Category>();
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public List<Category> Categories { get; set; }
        public FilePath Picture { get; set; }
    }
}
