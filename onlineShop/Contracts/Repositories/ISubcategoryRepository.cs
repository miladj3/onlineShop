using System.Collections.Generic;
using onlineShop.Models;

namespace onlineShop.Repositories
{
    public interface ISubcategoryRepository
    {
        void AddProductDescField(ProductDescriptionField descField);
        void AddSubcategory(Subcategory subcategory);
        ProductDescriptionField GetProductDescFieldById(int id);
        List<ProductDescriptionField> GetProductDescFieldsBySubcat(int subcategoryId);
        Subcategory GetSubcategoryById(int id);
        Subcategory GetSubcategoryInclById(int id);
        void RemoveProductDescField(ProductDescriptionField descField);
        void RemoveSubcategory(Subcategory subcategory);
        int SaveChanges();
        bool SubcategoryHasProducts(int id);
    }
}