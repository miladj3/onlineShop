using onlineShop.Models;

namespace onlineShop.Repositories
{
    public interface ICategoryRepository
    {
        void AddCategory(Category category);
        bool CategoryHasProducts(int id);
        Category GetCategoryById(int id);
        Category GetCategoryInclById(int id);
        void RemoveCategory(Category category);
        int SaveChanges();
    }
}