using Microsoft.AspNetCore.Http;
using onlineShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.ViewModels
{
    public class SubcategoryViewModel 
    {
        public SubcategoryViewModel()
        {
            Products = new List<Product>();
        }

        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description{ get; set; }

        public IFormFile PictureToUpload { get; set; }
        public FilePath Picture { get; set; }
        public List<Product> Products { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}
