using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public partial class CategoryController : Controller
    {
        private readonly IFileUploader _uploader;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryRepository _categoryController;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAuditTrailService _auditTrailService;

        public CategoryController(
            UserManager<AppUser> userManager,
            IFileUploader uploader,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            ICategoryRepository catalogRepository,
            IDepartmentRepository departmentRepository,
            IAuditTrailService auditTrailService)
        {
            _userManager = userManager;
            _uploader = uploader;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _categoryController = catalogRepository;
            _departmentRepository = departmentRepository;
            _auditTrailService = auditTrailService;
        }

        [HttpGet("/Catalog/Category/{id}")]
        public IActionResult Display(int id)
        {
            var category = _categoryController.GetCategoryById(id);

            var vm = new CategoryViewModel()
            {
                Subcategories = category.Subcategories,
                DepartmentId = category.DepartmentId,
                Description = category.Description,
                Id = category.Id,
                Name = category.Name,
            };

            PrepareNavData(id, "CategoryDisplay");

            return View(vm);
        }

        private void PrepareNavData(int categoryId, string nodeName)
        {

            var cat = _categoryController.GetCategoryById(categoryId);
            var dep = _departmentRepository.GetDepartmentById(cat.DepartmentId);

            _breadcrumbNavBuilder.CreateForNode(nodeName,
                new
                {
                    categoryId = cat.Id,
                    categoryName = cat.Name,
                    departmentId = dep.Id,
                    departmentName = dep.Name
                }
                , this);
        }
    }
}
