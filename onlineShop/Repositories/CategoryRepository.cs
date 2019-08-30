using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Linq;

namespace onlineShop.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories
                .Include(c => c.Subcategories)
                .ThenInclude(s => s.Picture)
                .FirstOrDefault(c => c.Id == id);
        }

        public Category GetCategoryInclById(int id)
        {
            return _context.Categories
                .Include(c => c.Subcategories)
                .ThenInclude(s => s.Picture)
                .Include(c => c.Subcategories)
                .ThenInclude(s => s.Products)
                .FirstOrDefault(d => d.Id == id);
        }

        public bool CategoryHasProducts(int id)
        {
            return _context.Products.Any(p => p.Subcategory.Category.Id == id);
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
        }

        public void RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
