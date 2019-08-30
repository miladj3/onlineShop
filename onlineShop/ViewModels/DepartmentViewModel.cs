using Microsoft.AspNetCore.Http;
using onlineShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.ViewModels
{
    public class DepartmentViewModel 
    {
        public DepartmentViewModel()
        {
            this.Categories = new List<Category>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description{ get; set; }

        public IFormFile PictureToUpload { get; set; }
        public FilePath Picture { get; set; }
        public List<Category> Categories { get; set; }

    }
}
