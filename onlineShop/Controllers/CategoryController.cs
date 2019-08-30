using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Filters;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public class CategoryController : Controller
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

        // ------------------------------------- ADMIN PANEL AREA: CATEGORY -------------------------------------------

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Categories/{id}")]
        public IActionResult AdminView(int id)
        {
            var category = _categoryController.GetCategoryInclById(id);

            PrepareNavData(id, "CPanelCategoryView");

            return View("AdminView", new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Picture = category.Picture,
                Subcategories = category.Subcategories,
                DepartmentId = category.DepartmentId
            });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Categories/Create")]
        public IActionResult Create(int departmentId)
        {
            // prepare breadcrumb navigation data
            var dep = _departmentRepository.GetDepartmentById(departmentId);

            _breadcrumbNavBuilder.CreateForNode("CPanelCategoryAdd",
                new
                {
                    departmentId = departmentId,
                    departmentName = dep.Name
                }
                , this);

            return View("Edit", new CategoryViewModel { DepartmentId = departmentId });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Categories/{id}/Edit/")]
        public IActionResult Edit(int id)
        {
            return View("Edit", FetchDataToModify(id));
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CategoryViewModel vm)
        {
            Category cat;
            string uploaderErrorMessage = "";

            // Validate all inputs
            if (vm.PictureToUpload != null)
            {
                if (!_uploader.ValidateImageSingle(vm.PictureToUpload, ref uploaderErrorMessage))
                    ModelState.AddModelError("", uploaderErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                if (vm.Id == 0)
                {
                    // prepare breadcrumb navigation data
                    var dep = _departmentRepository.GetDepartmentById(vm.DepartmentId);

                    _breadcrumbNavBuilder.CreateForNode("CPanelCategoryAdd",
                        new
                        {
                            departmentId = dep.Id,
                            departmentName = dep.Name
                        }
                        , this);

                    return View("Edit", new CategoryViewModel { DepartmentId = vm.DepartmentId });
                }
                else
                {
                    return View("Edit", FetchDataToModify(vm.Id));
                }
            }

            // New department
            if (vm.Id == 0)
            {
                cat = new Category
                {
                    Name = vm.Name,
                    Description = vm.Description,
                    DepartmentId = vm.DepartmentId
                };

                _categoryController.AddCategory(cat);
            }
            // Edit existing department
            else
            {
                cat = _categoryController.GetCategoryById(vm.Id);

                cat.Name = vm.Name;
                cat.Description = vm.Description;
            }

            // Set new image
            if (vm.PictureToUpload != null)
            {
                var imgUploaded = await _uploader.UploadSingle(vm.PictureToUpload);
                cat.Picture = new FilePath { Path = imgUploaded };
            }

            _auditTrailService.RetrieveAndLogChanges();

            _categoryController.SaveChanges();
            return RedirectToAction("AdminView", new { id = cat.Id });
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin")]
        [HttpPost("/ControlPanel/Categories/{id}/Delete")]
        public ActionResult Delete(int id)
        {
            var cat = _categoryController.GetCategoryById(id);

            if (_categoryController.CategoryHasProducts(id))
                return BadRequest("Action blocked: category contains some products.");

            _categoryController.RemoveCategory(cat);

            _auditTrailService.RetrieveAndLogChanges();

            _categoryController.SaveChanges();

            return Ok(new { redirectUrl = Url.Action("AdminView", "Department", new { id = cat.DepartmentId}) });
        }

        private CategoryViewModel FetchDataToModify(int id)
        {
            var category = _categoryController.GetCategoryById(id);

            PrepareNavData(id, "CPanelCategoryEdit");

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Picture = category.Picture,
                DepartmentId = category.DepartmentId
            };
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
