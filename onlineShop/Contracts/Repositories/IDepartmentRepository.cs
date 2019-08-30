using System.Collections.Generic;
using onlineShop.Models;

namespace onlineShop.Repositories
{
    public interface IDepartmentRepository
    {
        void AddDepartment(Department department);
        bool DepartmentHasProducts(int id);
        List<Department> GetAllDepartments();
        Department GetDepartmentById(int id);
        Department GetDepartmentInclById(int id);
        void RemoveDepartment(Department department);
        int SaveChanges();
    }
}