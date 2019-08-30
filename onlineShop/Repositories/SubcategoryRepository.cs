using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace onlineShop.Repositories
{
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public SubcategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Subcategory GetSubcategoryById(int id)
        {
            return _context.Subcategories
                .FirstOrDefault(s => s.Id == id);
        }

        public Subcategory GetSubcategoryInclById(int id)
        {
            return _context.Subcategories
                .Include(s => s.Picture)
                .Include(s => s.Products)
                .ThenInclude(p => p.Pictures)
                .Include(s => s.DescriptionFields)
                .FirstOrDefault(s => s.Id == id);
        }

        public bool SubcategoryHasProducts(int id)
        {
            return _context.Products.Any(p => p.SubcategoryId == id);
        }

        public void AddSubcategory(Subcategory subcategory)
        {
            _context.Subcategories.Add(subcategory);
        }

        public void RemoveSubcategory(Subcategory subcategory)
        {
            _context.Subcategories.Remove(subcategory);
        }

        public ProductDescriptionField GetProductDescFieldById(int id)
        {
            return _context.ProductDescriptionFields.FirstOrDefault(f => f.Id == id);
        }

        public List<ProductDescriptionField> GetProductDescFieldsBySubcat(int subcategoryId)
        {
            return _context.ProductDescriptionFields.AsNoTracking()
                .Where(f => f.SubcategoryId == subcategoryId)
                .OrderBy(f => f.DisplayOrderId)
                .ToList();
        }

        public void AddProductDescField(ProductDescriptionField descField)
        {
            _context.ProductDescriptionFields.Add(descField);
        }

        public void RemoveProductDescField(ProductDescriptionField descField)
        {
            _context.ProductDescriptionFields.Remove(descField);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
