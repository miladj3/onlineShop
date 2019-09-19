using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public partial class DepartmentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileUploader _uploader;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAuditTrailService _auditTrailService;

        public DepartmentController(
            UserManager<AppUser> userManager,
            IFileUploader uploader,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            IDepartmentRepository catalogRepository,
            IAuditTrailService auditTrailService)
        {
            _userManager = userManager;
            _uploader = uploader;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _departmentRepository = catalogRepository;
            _auditTrailService = auditTrailService;
        }

        [Route("/Catalog/Department/{id}")]
        public IActionResult Display(int id)
        {
            var department = _departmentRepository.GetDepartmentById(id);

            PrepareNavData(id, "DepartmentDisplay");

            var vm = new DepartmentViewModel()
            {
                Id = department.Id,
                Categories = department.Categories,
                Description = department.Description,
                Name = department.Name
            };

            return View(vm);
        }

        private void PrepareNavData(int departmentId, string nodeName)
        {

            var dep = _departmentRepository.GetDepartmentById(departmentId);

            _breadcrumbNavBuilder.CreateForNode(nodeName,
                new
                {
                    departmentId = dep.Id,
                    departmentName = dep.Name
                }
                , this);
        }
    }
}