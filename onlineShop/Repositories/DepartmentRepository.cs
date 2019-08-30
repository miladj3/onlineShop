using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace onlineShop.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Department> GetAllDepartments()
        {
            return _context.Departments.AsNoTracking()
                .Include(d => d.Categories)
                .Include(d => d.Picture)
                .ToList();
        }

        public Department GetDepartmentById(int id)
        {
            return _context.Departments
                .Include(d => d.Categories)
                .ThenInclude(c => c.Picture)
                .FirstOrDefault(d => d.Id == id);
        }

        public Department GetDepartmentInclById(int id)
        {
            return _context.Departments
                .Include(d => d.Categories)
                .ThenInclude(c => c.Picture)
                .Include(d => d.Categories)
                .ThenInclude(c => c.Subcategories)
                .FirstOrDefault(d => d.Id == id);
        }

        public bool DepartmentHasProducts(int id)
        {
            return _context.Products.Any(p => p.Subcategory.Category.Department.Id == id);
        }

        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
        }

        public void RemoveDepartment(Department department)
        {
            _context.Departments.Remove(department);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
