using onlineShop.Models;
using System.Collections.Generic;

namespace onlineShop.ViewModels
{
    public class ProductDescriptionFieldsViewModel
    {
        public ProductDescriptionFieldsViewModel()
        {
            this.Fields = new List<ProductDescriptionField>();
            this.FieldsToAdd = new List<ProductDescriptionField>();
        }
        public int SubcategoryId { get; set; }
        public List<ProductDescriptionField> Fields { get; set; }
        public List<ProductDescriptionField> FieldsToAdd { get; set; }

        public string CategoryName { get; set; }

    }
}
